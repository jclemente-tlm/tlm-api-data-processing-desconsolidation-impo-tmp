terraform {
  required_version = ">= 1.6.0"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    }
  }

  backend "s3" {

  }
}

provider "aws" {
  region = var.region
}

# ======================================================
# ECS CLUSTER EXISTENTE (no se crea uno nuevo)
# ======================================================
data "aws_ecs_cluster" "existing" {
  cluster_name = var.ecs_cluster_name
}
