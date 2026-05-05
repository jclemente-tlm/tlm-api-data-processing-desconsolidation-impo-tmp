output "ecs_cluster_arn" {
  description = "ARN del cluster ECS existente"
  value       = data.aws_ecs_cluster.existing.arn
}

output "ecs_task_definition_arn" {
  description = "ARN de la definición de tarea creada"
  value       = aws_ecs_task_definition.app.arn
}

output "ecs_task_family" {
  description = "Nombre de la familia de la tarea"
  value       = aws_ecs_task_definition.app.family
}

output "ecs_service_name" {
  description = "Nombre del servicio ECS creado"
  value       = aws_ecs_service.app_service.name
}
