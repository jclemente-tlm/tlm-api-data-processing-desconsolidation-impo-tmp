resource "aws_lb_target_group" "app" {
  name        = "${local.service_name}-${var.branch}-tg"
  port        = var.port_docker
  protocol    = "TCP"
  target_type = "ip"
  vpc_id      = var.vpc_id

  health_check {
    protocol            = "HTTP"
    port                = var.port_docker
    path                = "/health"
    healthy_threshold   = 3
    unhealthy_threshold = 3
    interval            = 30
  }

  tags = {
    ManagedBy = "Terraform"
  }
}

resource "aws_lb_listener" "app" {
  load_balancer_arn = var.nlb_arn
  port              = var.port_docker
  protocol          = "TCP"

  default_action {
    type             = "forward"
    target_group_arn = aws_lb_target_group.app.arn
  }

  tags = {
    ManagedBy = "Terraform"
  }
}

resource "aws_ecs_service" "app_service" {
  name            = "${local.service_name}-${var.branch}"
  cluster         = var.ecs_cluster_name
  task_definition = aws_ecs_task_definition.app.arn
  desired_count   = 1
  launch_type     = "FARGATE"

  network_configuration {
    subnets          = var.subnet_ids
    security_groups  = [var.security_group_id]
    assign_public_ip = false
  }

  deployment_controller {
    type = "ECS"
  }

  load_balancer {
    target_group_arn = aws_lb_target_group.app.arn
    container_name   = "${local.service_name}-${var.branch}"
    container_port   = var.port_docker
  }

  depends_on = [
    aws_ecs_task_definition.app,
    aws_lb_listener.app
  ]

  tags = {
    Name      = "dotnet-backend-service"
    ManagedBy = "Terraform"
  }
}