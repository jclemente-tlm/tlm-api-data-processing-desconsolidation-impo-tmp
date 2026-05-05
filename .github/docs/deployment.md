# Proceso de Despliegue

---

## Prerequisito — Bootstrap de workflows

> Aplica únicamente a repositorios nuevos donde `main` aún no tiene los archivos de `.github/workflows/`.

GitHub Actions requiere que los workflows existan en `main` para que `workflow_run` y `workflow_dispatch` funcionen. Este bootstrap es un paso único por repositorio.

### Pasos

**1. Crear la rama de bootstrap desde `main`**

```bash
git checkout main
git pull origin main
git checkout -b bootstrap/workflows
```

**2. Traer solo la carpeta `.github/` desde `feature/devops`**

```bash
git checkout feature/devops -- .github/
```

**3. Commit y push**

```bash
git add .github/
git commit -m "chore: bootstrap workflows y configuración CI/CD"
git push origin bootstrap/workflows
```

**4. Abrir PR** — `infra-team`

- Crear un PR de `bootstrap/workflows` → `main`
- Descripción: _Bootstrap inicial — agrega workflows de CI/CD y configuración de GitHub. No incluye código de aplicación._

**5. Revisar y aprobar el PR** — `devops-team` + `qa-team`

- Al menos 1 miembro de `devops-team` revisa y aprueba
- Al menos 1 miembro de `qa-team` revisa y aprueba

**6. Merge del PR** — `infra-team`

- Confirmar en el PR que las aprobaciones requeridas están completas
- Hacer merge del PR

**7. Validar**

- Hacer un merge a `dev` y confirmar en la pestaña **Actions** que el pipeline CD se dispara automáticamente

---

## Flujo general

```
dev (automático) → qa (manual) → prd (manual)
```

La misma imagen Docker se promueve entre ambientes sin recompilar. Solo cambia el tag: `vX.Y.Z-dev` → `vX.Y.Z-rc` → `vX.Y.Z`.

---

## Despliegue a `dev` — Automático

No requiere intervención. Al hacer merge de un PR a `dev`, el pipeline CI/CD se ejecuta automáticamente y despliega en ECS.

---

## Despliegue a `qa`

**Roles involucrados:** `dev-team` o `tech-leads` (abre el PR) · `devops-team` + `qa-team` (revisan y aprueban) · `infra-team` (hace merge y ejecuta el pipeline)

### Pasos

**1. Acceder a GitHub**

- Ingresar a [GitHub](https://github.com)
- Navegar al repositorio: `DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo`

**2. Abrir PR** — `dev-team` o `tech-leads`

- Crear un PR de `dev` → `qa`
- Indicar en la descripción la versión a promover (ej: `v1.2.3`) y los cambios incluidos

**3. Revisar y aprobar el PR** — `devops-team` + `qa-team`

- Ir a la pestaña **Pull requests** del repositorio y abrir el PR correspondiente
- Al menos 1 miembro de `devops-team` revisa y aprueba
- Al menos 1 miembro de `qa-team` revisa y aprueba

**4. Merge del PR** — `infra-team`

- Confirmar en el PR que las aprobaciones requeridas están completas
- Hacer merge del PR

**5. Ejecutar el pipeline CD** — `infra-team`

- Ir a GitHub → **Actions** → **CD** → **Run workflow**
- Completar los campos:
  - **environment:** `qa`
  - **version:** versión a desplegar (ej: `v1.2.3`)
- Confirmar que el pipeline finaliza exitosamente en la pestaña **Actions**

---

## Despliegue a `prd`

**Roles involucrados:** `tech-leads` (abre el PR) · `devops-team` + `qa-team` (revisan y aprueban) · `infra-team` (hace merge y ejecuta el pipeline)

> Prerequisito: la versión debe haber sido desplegada y validada previamente en `qa`.

### Pasos

**1. Acceder a GitHub**

- Ingresar a [GitHub](https://github.com)
- Navegar al repositorio: `DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo`

**2. Abrir PR** — `tech-leads`

- Crear un PR de `qa` → `main`
- Indicar en la descripción la versión a promover (ej: `v1.2.3`) y confirmar que fue validada en `qa`

**3. Revisar y aprobar el PR** — `devops-team` + `qa-team`

- Ir a la pestaña **Pull requests** del repositorio y abrir el PR correspondiente
- Al menos 1 miembro de `devops-team` revisa y aprueba
- Al menos 1 miembro de `qa-team` revisa y aprueba

**4. Merge del PR** — `infra-team`

- Confirmar en el PR que las aprobaciones requeridas están completas
- Hacer merge del PR

**5. Ejecutar el pipeline CD** — `infra-team`

- Ir a GitHub → **Actions** → **CD** → **Run workflow**
- Completar los campos:
  - **environment:** `prd`
  - **version:** versión a desplegar (ej: `v1.2.3`)
- Confirmar que el pipeline finaliza exitosamente en la pestaña **Actions**

---

## Notas

- **Versión requerida para `qa` y `prd`:** debe existir previamente en ECR (generada en un despliegue a `dev`). Formato: `vX.Y.Z`.

---

## Archivos relacionados

- [`.github/workflows/ci.yml`](../workflows/ci.yml) — pipeline CI
- [`.github/workflows/cd.yml`](../workflows/cd.yml) — orquestador CD
- [`.github/workflows/_publish.yml`](../workflows/_publish.yml) — build y publicación de imagen
- [`.github/workflows/_deploy.yml`](../workflows/_deploy.yml) — despliegue en ECS via Terraform
- [`iac/`](../../iac/) — infraestructura como código (Terraform)
