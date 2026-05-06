resource "aws_ecs_task_definition" "app" {
  family                   = "${local.service_name}-${var.branch}"
  requires_compatibilities = ["FARGATE"]
  network_mode             = "awsvpc"
  cpu                      = "256"
  memory                   = "512"

  execution_role_arn = aws_iam_role.ecs_task_execution.arn
  task_role_arn      = aws_iam_role.ecs_task_role.arn

  container_definitions = jsonencode([
    {
      name      = "${local.service_name}-${var.branch}"
      image     = "${var.image_name}:${var.image_tag}"
      essential = true

      portMappings = [
        {
          containerPort = var.port_docker
          hostPort      = var.port_docker
          protocol      = "tcp"
        }
      ]

      environment = [
        {
          name  = "ASPNETCORE_HTTP_PORTS"
          value = tostring(var.port_docker)
        }
      ]

      logConfiguration = {
        logDriver = "awsfirelens"
        options = {
          Name   = "loki"
          Host   = var.loki_host
          Port   = "3100"
          labels = "service=${local.service_name}-${var.branch}"
        }
      }
    },
    {
      name      = "log_router"
      image     = "public.ecr.aws/aws-observability/aws-for-fluent-bit:stable"
      essential = false # revertir a true cuando Loki esté accesible
      firelensConfiguration = {
        type = "fluentbit"
      }
    }
  ])

  depends_on = [
    aws_iam_role_policy_attachment.ecs_task_execution_policy,
    aws_iam_role_policy_attachment.ecs_secrets_access
  ]

  lifecycle {
    create_before_destroy = true
  }

  tags = {
    Name      = "dotnet-backend-task"
    ManagedBy = "Terraform"
  }
}