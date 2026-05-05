ecs_cluster_name  = "ai-extracciones-prd"
branch            = "prd"
subnet_ids        = ["subnet-0e48619365e1b6465", "subnet-09da616e84072380e"]
security_group_id = "sg-0987c9928e122edd5"
vpc_id            = "vpc-0d9fda7425fe38aac"
nlb_arn           = "" # TODO: completar con ARN del NLB de PRD
port_docker       = 5010