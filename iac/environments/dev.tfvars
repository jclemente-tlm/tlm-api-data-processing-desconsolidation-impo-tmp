ecs_cluster_name  = "ai-extracciones-dev"
branch            = "dev"
subnet_ids        = ["subnet-00b929846c486a95a", "subnet-07cf5c5ff9a54858f"]
security_group_id = "sg-0a1a841db315a0fd7"
vpc_id            = "vpc-0e10adcc94ccd6115"
nlb_arn           = "arn:aws:elasticloadbalancing:us-east-1:122083690056:loadbalancer/net/NLB-aiservices-dev/d1722e95639615c7"
port_docker       = 5010