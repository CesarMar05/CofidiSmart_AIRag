# CHANGELOG

Este documento describe los cambios relevantes realizados en la solución a lo largo del tiempo.  
El formato sigue las recomendaciones de [Keep a Changelog](https://keepachangelog.com/)  
y el control de versiones se basa en [Semantic Versioning](https://semver.org/).

---

## [0.1.2.0 MVP] 2025-10-23
### Added
- Se agrega EndPoint de para establecer el promopt para ApplicationClient

---

## [0.1.1 MVP]
### Added
- Se agreaga Prompt a ApplicationClient para poder personalizar el Prompt por Aplicación.
- Se agrega Controlador de Migrations para crear y actulizar la base de datos.
- Se agrega Controlador de Version que lista las versiones de los componentes del API.
- Se crea Migración Change03.

### Changed
- Se mueven interfaces de Repository de Application.Abstraction a Application
- Se actualiza RespositoryBase para ser mas general.

---

## [0.1.0 MVP] - 2025-07-24
### Added
- MVP Versión inicial: estructura básica, carga de documentos, generación de embeddings y consulta a modelo vía LangChain.

---

## [Unreleased]
### Added
- (Ejemplo) Nueva funcionalidad agregada pero no liberada oficialmente.

### Changed
- (Ejemplo) Modificación en comportamiento existente pendiente de release.

### Fixed
- (Ejemplo) Corrección de errores detectados durante pruebas internas.

### Deprecated
- (Ejemplo) Elementos que quedarán obsoletos en futuras versiones.

### Removed
- (Ejemplo) Funcionalidades eliminadas o reemplazadas.

### Security
- (Ejemplo) Ajustes relacionados con seguridad o dependencias críticas.

---

## Convenciones de Versionado

El esquema de versionado sigue **MAJOR.MINOR.PATCH**:
- **MAJOR**: Cambios incompatibles o refactorización arquitectónica.
- **MINOR**: Nuevas funcionalidades compatibles hacia atrás.
- **PATCH**: Corrección de errores o mejoras menores.

---

## Procedimiento de Actualización del CHANGELOG

1. Cada *pull request* o *merge request* aprobado debe incluir una actualización en `[Unreleased]`.
2. Antes de generar un nuevo *tag*, mover los cambios a una nueva versión con número y fecha.
3. Confirmar que los cambios estén asociados a un commit Git y versión del proveedor (`docs/VERSIONING.md`).
4. Crear el *tag* en Git correspondiente a la versión liberada.

---

## Ejemplo de Publicación

```bash
# Crear y subir nuevo tag
git tag -a v1.5.0 -m "Release 1.5.0 - Integración extendida con Azure AD"
git push origin v1.5.0