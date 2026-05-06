## [1.0.2](https://github.com/jclemente-tlm/tlm-api-data-processing-desconsolidation-impo-tmp/compare/v1.0.1...v1.0.2) (2026-05-06)


### Bug Fixes

* actualiza el flujo de trabajo de release y CD para soportar la rama de entrada ([8270779](https://github.com/jclemente-tlm/tlm-api-data-processing-desconsolidation-impo-tmp/commit/827077934ce0083f07c0f2952043623fb9c0726a))
* actualiza la acción de generación de release para manejar correctamente la publicación y la versión ([ab29703](https://github.com/jclemente-tlm/tlm-api-data-processing-desconsolidation-impo-tmp/commit/ab297036a531448f6b6f9c6c7d2c43f6b4967203))
* añade la variable GITHUB_REF_NAME para mejorar la gestión de ramas en el flujo de trabajo de release ([0b0d115](https://github.com/jclemente-tlm/tlm-api-data-processing-desconsolidation-impo-tmp/commit/0b0d11501691275a0920ef6fd20f0b855dadc93b))
* añade paso de depuración para la entrada de rama en el flujo de trabajo de release ([03451a6](https://github.com/jclemente-tlm/tlm-api-data-processing-desconsolidation-impo-tmp/commit/03451a662e24f23d12185a1f06b4162f3e93e8a8))
* corrige la salida de depuración para la entrada de rama en el flujo de trabajo de release ([8bc7f56](https://github.com/jclemente-tlm/tlm-api-data-processing-desconsolidation-impo-tmp/commit/8bc7f562b70ffcd134cbe3ac62d37a98fe2cc0f8))
* elimina la duplicación del título en el README ([6ff1fbe](https://github.com/jclemente-tlm/tlm-api-data-processing-desconsolidation-impo-tmp/commit/6ff1fbe0245a2ee0df42e700825fab637199e959))
* simplifica la generación de release eliminando el código redundante y ajustando la configuración de plugins ([2469098](https://github.com/jclemente-tlm/tlm-api-data-processing-desconsolidation-impo-tmp/commit/24690986492b9181bce1db7dbf48d7619f600cec))

## [1.0.1](https://github.com/jclemente-tlm/tlm-api-data-processing-desconsolidation-impo-tmp/compare/v1.0.0...v1.0.1) (2026-05-05)


### Bug Fixes

* elimina la entrada de rama en el flujo de trabajo de release y establece ambiente por defecto en dev ([a756606](https://github.com/jclemente-tlm/tlm-api-data-processing-desconsolidation-impo-tmp/commit/a7566065a80b4a9e52f2ee1c3a3ffcbae424b544))

# 1.0.0 (2026-05-05)


### Bug Fixes

* actualiza el flujo de trabajo de release para usar la rama en lugar del SHA del commit ([ae02451](https://github.com/jclemente-tlm/tlm-api-data-processing-desconsolidation-impo-tmp/commit/ae024510d82b5383f1a7e66c094f91c8ffb67bd5))
* actualiza el nombre del paquete en package-lock.json ([2d60c4f](https://github.com/jclemente-tlm/tlm-api-data-processing-desconsolidation-impo-tmp/commit/2d60c4f4097dccc4fde2c89a5feefe12309b4d93))
* añade referencia para el checkout en el flujo de trabajo de release ([f0c4df0](https://github.com/jclemente-tlm/tlm-api-data-processing-desconsolidation-impo-tmp/commit/f0c4df04ebe8cfaf2ecc21f5d33f3bd861731ddd))
* añade soporte para la referencia de rama en el flujo de trabajo de CD ([b711f7f](https://github.com/jclemente-tlm/tlm-api-data-processing-desconsolidation-impo-tmp/commit/b711f7f59cdae4f196e212f2bbd0d0cf174d4d51))
* añade soporte para procesar SHA de commit en el flujo de trabajo de release ([b28c186](https://github.com/jclemente-tlm/tlm-api-data-processing-desconsolidation-impo-tmp/commit/b28c186ce40af39e35be5f39f4683d085f297091))
* añade soporte para procesar SHA de commit en el flujo de trabajo de release ([80f1d83](https://github.com/jclemente-tlm/tlm-api-data-processing-desconsolidation-impo-tmp/commit/80f1d83a902127e1f6377c56b79950dca4c908a6))
* corrige la configuración de checkout para usar el token de GitHub ([859e61a](https://github.com/jclemente-tlm/tlm-api-data-processing-desconsolidation-impo-tmp/commit/859e61a21c1d1d714096dceeb2032de27b1dbf13))
* desactiva etapas de publicación y despliegue en el flujo de trabajo de CD ([e7e8989](https://github.com/jclemente-tlm/tlm-api-data-processing-desconsolidation-impo-tmp/commit/e7e898931f233ab67a09c0780b10ca04f6204988))
* elimina la entrada de SHA de commit en el flujo de trabajo de release ([d34c6d7](https://github.com/jclemente-tlm/tlm-api-data-processing-desconsolidation-impo-tmp/commit/d34c6d774d2af728420f715d41d1bd2a7fbe94a5))
* elimina versión específica de curl en Dockerfile ([29c6780](https://github.com/jclemente-tlm/tlm-api-data-processing-desconsolidation-impo-tmp/commit/29c678081c443559bb485e6906e64b483ef19c80))
* especifica la versión de curl en Dockerfile ([f12d5f4](https://github.com/jclemente-tlm/tlm-api-data-processing-desconsolidation-impo-tmp/commit/f12d5f4341a7a1b678d7778e9ec42f3c14c76462))


### Features

* añade proyecto inicial ([06c03ec](https://github.com/jclemente-tlm/tlm-api-data-processing-desconsolidation-impo-tmp/commit/06c03ec8d559c5bbb891572b7d9a6e3b4f9bffb9))

## [1.5.4](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/compare/v1.5.3...v1.5.4) (2026-04-30)


### Bug Fixes

* eliminar la configuración de lifecycle para ignorar cambios en task_definition ([4f57e31](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/4f57e31c82a8e52318ba89cbaa6e332be05f941f))

## [1.5.3](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/compare/v1.5.2...v1.5.3) (2026-04-30)


### Bug Fixes

* establecer 'essential' en false para el contenedor log_router hasta que Loki esté accesible ([0983c8d](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/0983c8d8f0a61a1cf554cf421d09931545a87b74))

## [1.5.2](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/compare/v1.5.1...v1.5.2) (2026-04-30)


### Bug Fixes

* eliminar la opción de omitir el despliegue en el flujo de trabajo de CD ([54453ee](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/54453ee757e2823c5c441f60e8282dbd40548b11))

## [1.5.1](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/compare/v1.5.0...v1.5.1) (2026-04-30)


### Bug Fixes

* actualizar la acción de configuración de credenciales AWS a la versión 6 en los flujos de trabajo de despliegue y publicación ([ef097e8](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/ef097e88cf516c2870ba9909bb2e1167e0382521))

# [1.5.0](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/compare/v1.4.1...v1.5.0) (2026-04-30)


### Bug Fixes

* actualizar el puerto de ASPNETCORE_HTTP_PORTS a 5000 y ajustar configuraciones en los archivos de entorno ([97bdfd3](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/97bdfd3ae4b031b05279feeb7106dfa5bb2ef314))
* corregir el nombre del grupo de destino en la configuración de AWS LB ([853c207](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/853c207896e6f8d27de76c0bedb0131eb201a8ce))


### Features

* agregar soporte para Health Checks en la configuración de la API ([5b07df9](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/5b07df9fbfbd381d3265f391c2a4fad39fad0b6c))
* agregar soporte para NLB y variables relacionadas en la configuración de ECS ([df558a9](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/df558a9a626b1166a9ce2cc974bb843dfe3055c6))

## [1.4.1](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/compare/v1.4.0...v1.4.1) (2026-04-30)


### Bug Fixes

* eliminar configuración de ignore_changes en la definición de tarea ECS y eliminar salida de rol de despliegue de GitHub Actions ([604fa3e](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/604fa3e563b4887f8a853945cbdf2c03693e6f70))

# [1.4.0](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/compare/v1.3.0...v1.4.0) (2026-04-30)


### Features

* agregar soporte para CD en pull_request hacia dev ([070a16c](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/070a16cf258fc68c25250da39779df77be6dd996))

# [1.3.0](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/compare/v1.2.0...v1.3.0) (2026-04-30)


### Features

* agregar variable tf_state_bucket en el flujo de trabajo de despliegue ([ca90b0e](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/ca90b0ecf3fb7e564ffcf5e78b5fd925f9de0d7b))

# [1.2.0](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/compare/v1.1.0...v1.2.0) (2026-04-30)


### Features

* actualizar la acción de configuración de Node.js a la versión 5 ([6992dca](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/6992dcad7db11cd4986aa7ffc44c15b73f388bf3))
* actualizar nombres de clúster ECS y configuraciones de red en entornos dev, prd y qa ([4f8f488](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/4f8f488d01cca57b9e01ccb1d6b378745f93194d))
* agregar configuración de concurrencia en los flujos de trabajo CI y CD ([3abad29](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/3abad293fbdf45fb4fdc0ff4a1f010d9d715bc15))
* agregar recurso de repositorio ECR y política de acceso entre cuentas ([ba66dce](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/ba66dcee339181ebfb169ac1cd8fb9f03cb6f017))
* agregar variable tf_state_bucket y actualizar configuración de Terraform en flujos de trabajo ([eaea17f](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/eaea17f8f7b09cbd9e62ff3f6d882f27d4a427da))
* eliminar configuración del repositorio ECR y su política de acceso ([f4452c6](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/f4452c6fd52938e146c7b605bc220ad5f89dafb3))
* hacer que el secreto HOST_LOKI no sea obligatorio en el flujo de trabajo de despliegue ([dff3d35](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/dff3d358e286039c363986060b5d95232efa01ef))
* renombrar variables de repositorio a nombre de servicio en la configuración de ECS ([2c1814f](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/2c1814fa1584d7f20d7466b848e7350a0284846b))

# [1.1.0](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/compare/v1.0.0...v1.1.0) (2026-04-29)


### Features

* actualizar la acción de Semantic Release a la versión 6 ([af78fde](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/af78fde58a0fb8f407ae9a63bb4adfb972ce955a))

# 1.0.0 (2026-04-29)


### Bug Fixes

* actualizar autenticación en las consultas a la API de SonarQube para usar Bearer Token ([716a63d](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/716a63ddfd6221952a0ad8e26223c8f87fa871fb))
* corregir nombre de secreto para la URL de SonarQube en el flujo de trabajo SAST ([c382667](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/c3826674f4980c325610978cdf20364a8a418dff))


### Features

* actualizar acciones de descarga y subida de artefactos a la versión más reciente ([5a11825](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/5a1182508d80765a8570358dc4815af27893d376))
* actualizar acciones de GitHub a versiones más recientes para mejorar la estabilidad y el rendimiento ([599a224](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/599a22448f62059ec7aff1c7da96dfcf28eabb3b))
* actualizar descripciones de ECR y agregar resolución de nombre de proyecto en flujos de trabajo de despliegue y publicación ([1f469b1](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/1f469b1df56c26ac1986a5523795670e140c1c43))
* actualizar flujos de trabajo de GitHub Actions para implementar despliegue en ECR y mejorar la gestión de credenciales ([12d7f88](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/12d7f8821d400d5d67485efb25d0072407c45d62))
* actualizar la versión de Node.js a 24 en el flujo de trabajo de Semantic Release ([2146420](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/2146420f2195960b22dcad02b4ff4b9f964bf09b))
* actualizar patrones de ramas en el flujo de trabajo CI para mayor precisión ([01a8ce0](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/01a8ce0094d59bf2b826e74d3fd1954538bb00e9))
* actualizar políticas de IAM para el rol de despliegue de GitHub Actions con recursos específicos ([0a8845d](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/0a8845d5a97dfe86e58dca65b2c33c05b1b6b00f))
* actualizar versión de curl en el Dockerfile para mejorar la estabilidad ([6b7f672](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/6b7f6726ca354e180f4dfa2d76540e0f1d4bd600))
* agregar configuración inicial para semantic-release en .releaserc.json y package.json ([0c5e542](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/0c5e542be209748743d1b3d8d658bca30a97965f))
* agregar excepciones para archivos de configuración en .gitignore ([60afc93](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/60afc934377ecbcd06e081fd2aedc182900c5731))
* agregar la rama 'dev' a los eventos de push en el flujo de trabajo de CI ([6b2bef7](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/6b2bef79b98dc8bc60e76dd4b9b9c2a04d5cc80e))
* agregar módulo de escaneo de secretos con Gitleaks al flujo de trabajo CI ([4a425bf](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/4a425bf0e4f93df3755c94429a686ddb31ccab97))
* agregar package-lock.json a la lista de archivos no ignorados ([78716ad](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/78716ada32ddb5c59cd489c33455ef9a7444e49a))
* Añadir workflows de commitlint, semantic release y despliegue a GHCR con mejoras en la lógica de despliegue ([7612627](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/76126279aeec48b5091c39ddf69e5560885ec15c))
* cambiar el desencadenador de CD a push en dev y ajustar la lógica de detección de ambiente ([cb05cd4](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/cb05cd481a16e8c95b9f79dd2f97dc45133e25d6))
* Configuración inicial de CI/CD y despliegue en AWS ECS ([06e1c68](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/06e1c681fc4fa04e9814c593f2414b99afc3a4cf))
* eliminar proyectos de pruebas de la solución para simplificar la estructura ([b815890](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/b815890ff408a520c04ca04c4ffac70169c76954))
* implementa flujos de trabajo CI/CD modulares para proyectos .NET ([96350e7](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/96350e79f16023b807e67522d7918f100ddf7c4e))
* initial project setup - Talma AI Services desconsolidación impo ([6a60731](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/6a60731933a2cdf1bf17585c42c0523ee9db2c02))
* Mejora en la lógica de despliegue y ajuste de etiquetas en los workflows de GitHub Actions ([a748c71](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/a748c718fee1cdbef4539c80df8c6213a845b01b))
* mejorar el análisis de vulnerabilidades en el reporte de OWASP añadiendo soporte para severidades medias y bajas ([3c0cbf2](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/3c0cbf2cbf05fa6ac5179b7949ea3cbe53e1b2cc))
* mejorar el análisis de vulnerabilidades en el reporte de Trivy ([6f088ba](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/6f088ba07a3b3f8d5951df1db26b078373756dab))
* mejorar la configuración del flujo de trabajo de CD para permitir validación manual y omitir despliegue ([37f5ed8](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/37f5ed85c2f53410165726cce35e3b808a6eac71))
* mejorar la validación de vulnerabilidades en el reporte de OWASP ([6edbb7d](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/6edbb7d839f9630497fa74681a1bebdbbe294bfc))
* Mejorar workflows de CI/CD con ajustes en la lógica de espera y formato de salida ([465643c](https://github.com/DigitalFactoryTalma/tlm-api-data-processing-desconsolidation-impo/commit/465643c40f0fc4e9379482a66d923289c622e5e9))
