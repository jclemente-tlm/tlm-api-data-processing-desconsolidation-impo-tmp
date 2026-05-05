# Reglas de Protección de Ramas (Branch Protection Rules)

> Ruta en GitHub: **Settings → Branches → Branch protection rules**

---

## Rama `dev` — Integración continua

| Configuración                                                          | Valor                                                                  |
| ---------------------------------------------------------------------- | ---------------------------------------------------------------------- |
| **Requerir un pull request antes de fusionar**                         | ✅ Activado                                                            |
| → Aprobaciones requeridas                                              | **1**                                                                  |
| → Descartar revisiones desactualizadas cuando se envíen nuevos commits | ✅ Activado                                                            |
| → Requerir revisión de los propietarios del código (Code Owners)       | ✅ Activado — aplica a `/iac/`, `/.github/workflows/`, `**/Dockerfile` |
| → No permitir commits de merge directo                                 | ✅ Usar Squash merge                                                   |
| **Requerir que los checks de estado pasen antes de fusionar**          | ✅ Activado                                   **                         |
| → Requerir que las ramas estén actualizadas                            | ✅ Activado                                                            |
| **Requerir resolución de conversaciones antes de fusionar**            | ✅ Activado                                                            |
| **Permitir force push**                                                | ❌ Desactivado                                                         |
| **Permitir eliminación de la rama**                                    | ❌ Desactivado                                                         |

### Status checks requeridos para `dev`

```
build
test
scan-sast
scan-secrets
scan-dockerfile
scan-deps
scan-trivy
scan-iac
```

### ¿Quién abre el PR?

- `dev-team` (desde `feature/*` o `fix/*`)

### ¿Quién revisa y aprueba?

- `tech-leads` — al menos 1 miembro
- `devops-team` — requerido automáticamente si el PR toca `/iac/`, `/.github/workflows/` o `**/Dockerfile`

### ¿Quién hace merge?

- `tech-leads` o `devops-team` (configurado en _Restringir quién puede hacer push_)

---

## Rama `qa` — Ambiente de pruebas

| Configuración                                                          | Valor                |
| ---------------------------------------------------------------------- | -------------------- |
| **Requerir un pull request antes de fusionar**                         | ✅ Activado          |
| → Aprobaciones requeridas                                              | **2**                |
| → Descartar revisiones desactualizadas cuando se envíen nuevos commits | ✅ Activado          |
| → Requerir que las ramas estén actualizadas                            | ✅ Activado          |
| **Requerir resolución de conversaciones antes de fusionar**            | ✅ Activado          |
| **Restringir quién puede hacer push a ramas coincidentes**             | ✅ Solo `infra-team` |
| **Permitir force push**                                                | ❌ Desactivado       |
| **Permitir eliminación de la rama**                                    | ❌ Desactivado       |

### ¿Quién abre el PR?

- `dev-team` o `tech-leads` (desde `dev`)

### ¿Quién revisa y aprueba?

- `devops-team` — al menos 1 miembro
- `qa-team` — al menos 1 miembro

### ¿Quién hace merge?

- `infra-team` (configurado en _Restringir quién puede hacer push_)

### Nota

Solo se aceptan PRs provenientes de la rama `dev`.

---

## Rama `main` — Producción

| Configuración                                                          | Valor                                 |
| ---------------------------------------------------------------------- | ------------------------------------- |
| **Requerir un pull request antes de fusionar**                         | ✅ Activado                           |
| → Aprobaciones requeridas                                              | **2**                                 |
| → Descartar revisiones desactualizadas cuando se envíen nuevos commits | ✅ Activado                           |
| → Requerir aprobación del push más reciente                            | ✅ Activado                           |
| **Requerir que los checks de estado pasen antes de fusionar**          | ✅ Activado                           |
| → Requerir que las ramas estén actualizadas                            | ✅ Activado                           |
| **Requerir historial lineal**                                          | ✅ Activado (evita merge commits)     |
| **Requerir resolución de conversaciones antes de fusionar**            | ✅ Activado                           |
| **No permitir omitir las configuraciones anteriores**                  | ✅ Activado — aplica también a admins |
| **Restringir quién puede hacer push a ramas coincidentes**             | ✅ Solo `infra-team`                  |
| **Permitir force push**                                                | ❌ Desactivado                        |
| **Permitir eliminación de la rama**                                    | ❌ Desactivado                        |

### ¿Quién abre el PR?

- `tech-leads` (desde `qa`)

### ¿Quién revisa y aprueba?

- `devops-team` — al menos 1 miembro
- `qa-team` — al menos 1 miembro (segunda aprobación requerida)

### ¿Quién hace merge?

- `infra-team` (configurado en _Restringir quién puede hacer push_)

---

## Eliminación automática de ramas tras merge

> Ruta en GitHub: **Settings → General → Pull Requests**

| Configuración                          | Valor       |
| -------------------------------------- | ----------- |
| **Automatically delete head branches** | ✅ Activado |

Elimina automáticamente la rama de origen (`feature/*`, `fix/*`) al fusionar el PR.

> **Importante:** Las ramas `dev`, `qa` y `main` deben tener **Permitir eliminación de la rama → ❌ Desactivado** en sus reglas de protección. Sin esa configuración, el auto-delete también las eliminaría al ser la rama origen de un PR (p. ej. `dev` → `qa`, `qa` → `main`).

---

## Resumen rápido

| Rama   | Aprueba                                             | Merge                        | Force Push |
| ------ | --------------------------------------------------- | ---------------------------- | :--------: |
| `dev`  | `tech-leads` (mín. 1) + `devops-team` si toca infra | `tech-leads` o `devops-team` |     ❌     |
| `qa`   | `devops-team` + `qa-team`                           | `infra-team`                 |     ❌     |
| `main` | `devops-team` + `qa-team`                           | `infra-team`                 |     ❌     |

---

## Teams y roles en el repositorio

> Ruta en GitHub: **Settings → Collaborators and teams**

| Team          | Rol          | Miembros                    | Para qué sirve                                                 |
| ------------- | ------------ | --------------------------- | -------------------------------------------------------------- |
| `dev-team`    | **Write**    | Developers                  | Push directo a `feature/*`, `fix/*`; abrir PRs                 |
| `qa-team`     | **Write**    | QA Engineers                | Revisar y aprobar PRs, no hacen push al repositorio            |
| `tech-leads`  | **Maintain** | Tech Leads                  | Revisar y aprobar PRs, merge a `dev` (junto con `devops-team`) |
| `devops-team` | **Maintain** | Arquitectos + Infra + Pases | Merge a `dev` — ejecutan deploys                               |
| `architects`  | **Write**    | 2 arquitectos de software   | Peer review de código cuando se requiera                       |
| `infra-team`  | **Maintain** | 1 arquitecto cloud          | Merge a `qa`, `main` — ejecutan deploys                        |

### Membresía múltiple

Un usuario puede pertenecer a varios teams. Así cada persona tiene el acceso correcto según su rol:

| Persona                     | Teams                        |
| --------------------------- | ---------------------------- |
| Arquitecto de software (x2) | `architects` + `devops-team` |
| Arquitecto cloud            | `infra-team` + `devops-team` |
| Pase de infra (x2)          | `devops-team`                |

---

## Archivos relacionados

- [`.github/CODEOWNERS`](../CODEOWNERS) — define quién debe revisar cambios por ruta
- [`.github/workflows/ci.yml`](../workflows/ci.yml) — pipeline CI con los status checks
- [`.github/workflows/cd.yml`](../workflows/cd.yml) — pipeline CD con environments y aprobaciones
