ecs_cluster_name  = "ai-extracciones-qa"
branch            = "qa"
subnet_ids        = ["subnet-00b929846c486a95a", "subnet-07cf5c5ff9a54858f"]
security_group_id = "sg-0308fec89ebcab816"
vpc_id            = "vpc-0e10adcc94ccd6115"
nlb_arn           = "arn:aws:elasticloadbalancing:us-east-1:122083690056:loadbalancer/net/nlb-aiservices-rc/80a24c5a28e1914f"
port_docker       = 5010