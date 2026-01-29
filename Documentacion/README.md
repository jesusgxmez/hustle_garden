# Documentación de Hustle Garden

Este directorio contiene la configuración y los archivos generados de la documentación del proyecto usando DocFX.

## Requisitos

- DocFX instalado globalmente: `dotnet tool install -g docfx`
- .NET 10 SDK

## Estructura

- `docfx.json` - Archivo de configuración de DocFX
- `docs/` - Documentación markdown personalizada
- `api/` - Documentación de API generada automáticamente (generada)
- `_site/` - Sitio web estático generado (generado)

## Comandos

### Generar documentación

```bash
docfx docfx.json
```

Este comando:
1. Extrae metadatos de los archivos XML de documentación de los proyectos
2. Genera archivos YAML con la API
3. Construye el sitio HTML estático en `_site/`
4. Genera PDFs de la documentación

### Ver documentación localmente

```bash
docfx serve _site
```

Luego abre tu navegador en: http://localhost:8080

### Generar y servir en un solo comando

```bash
docfx docfx.json --serve
```

## Actualizar documentación

Después de hacer cambios en los comentarios XML del código:

1. Compila el proyecto para generar los archivos XML actualizados
2. Ejecuta `docfx docfx.json` para regenerar la documentación
3. Los cambios se reflejarán en `_site/`

## Archivos generados

Los siguientes directorios/archivos son generados automáticamente y no deben ser incluidos en el control de versiones:

- `api/`
- `_site/`
- `obj/`

## Configuración

El archivo `docfx.json` está configurado para:

- **src**: Buscar proyectos `.csproj` en el directorio padre (`..`)
- **output**: Generar el sitio en `_site/`
- **templates**: Usar las plantillas `default` y `modern`
- **features**: Búsqueda habilitada, PDF habilitado

## Personalización

Puedes personalizar la documentación editando:

- `docs/index.md` - Página de inicio
- `docfx.json` - Configuración de DocFX
- Archivos markdown adicionales en `docs/`

## Notas

- Los archivos XML de documentación se generan automáticamente durante la compilación gracias a `<GenerateDocumentationFile>true</GenerateDocumentationFile>` en los archivos `.csproj`
- Los comentarios XML en el código fuente (con `///`) se extraen para generar la documentación de la API
