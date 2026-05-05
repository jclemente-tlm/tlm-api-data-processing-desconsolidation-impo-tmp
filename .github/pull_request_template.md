## Descripción

<!-- Qué cambia y por qué. Enlaza el issue relacionado si aplica. -->

Closes #

---

## Tipo de cambio

<!-- Marca con una X lo que aplique -->

- [ ] 🐛 Bug fix
- [ ] ✨ Nueva funcionalidad
- [ ] 🔧 Refactor / mejora técnica
- [ ] 🏗️ Infraestructura / DevOps
- [ ] 📝 Documentación

---

## Checklist

### Código

- [ ] El código compila sin errores (`dotnet build`)
- [ ] Los tests pasan (`dotnet test`)
- [ ] No se incluyen secrets, tokens ni credenciales hardcodeadas
- [ ] Los cambios están cubiertos por tests (si aplica)

### Infraestructura (IaC) — completar solo si se modificó `/iac`

- [ ] `terraform fmt` ejecutado
- [ ] `terraform validate` sin errores
- [ ] Checkov no reporta fallos críticos
- [ ] Los `.tfvars` de entorno están actualizados

### CI/CD — completar solo si se modificaron workflows

- [ ] El workflow fue probado en la rama actual
- [ ] No se exponen secrets en logs (`echo $SECRET` u otros)

---

## Evidencia / capturas

- [ ] El workflow fue probado en la rama actual
- [ ] No se exponen secrets en logs (`echo $SECRET` u otros)

---

## Evidencia / capturas
<!-- Screenshots, logs relevantes o links a runs de GitHub Actions -->
