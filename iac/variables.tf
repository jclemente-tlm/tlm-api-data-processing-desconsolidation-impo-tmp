variable "image_name" {
  description = "URI completa del repositorio ECR (ej: 123456789012.dkr.ecr.us-east-1.amazonaws.com/mi-repo)"
  type        = string
}

locals {
  service_name = lower(var.service_name)
  # Extrae el account ID desde la URI de ECR: "111111111111.dkr.ecr.us-east-1.amazonaws.com/repo"
  ecr_account_id = split(".", var.image_name)[0]
  # Extrae el nombre del repositorio desde la URI de ECR: "111111111111.dkr.ecr.us-east-1.amazonaws.com/repo"
  ecr_repo_name = split("/", var.image_name)[1]
}

variable "service_name" {
  description = "Nombre del servicio (ej: ai-extracciones-api)"
  type        = string
}

variable "image_tag" {
  description = "Tag o versión semver de la imagen (por ejemplo: v1.0.3)"
  type        = string
}

variable "loki_host" {
  description = "IP o hostname del servidor Loki"
  type        = string
}

variable "region" {
  description = "Región AWS donde se desplegará (ej. us-east-1)"
  type        = string
  default     = "us-east-1"
}

variable "port_docker" {
  description = "Puerto expuesto por la aplicación en el contenedor Docker"
  type        = number
  default     = 5000

}

variable "ecs_cluster_name" {
  description = "Nombre del cluster ECS existente"
  type        = string
}

variable "branch" {
  description = "Nombre de la rama/ambiente de despliegue (dev, qa, prd)"
  type        = string
}

variable "subnet_ids" {
  description = "Lista de IDs de subnets donde se ejecutará la tarea ECS"
  type        = list(string)
}

variable "security_group_id" {
  description = "ID del Security Group asignado al servicio ECS"
  type        = string
}

variable "vpc_id" {
  description = "ID de la VPC donde se desplegará el servicio"
  type        = string
}

variable "nlb_arn" {
  description = "ARN del Network Load Balancer interno compartido"
  type        = string
}
