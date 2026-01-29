# Hustle Garden - Documentación

Bienvenido a la documentación de **Hustle Garden**, una aplicación .NET MAUI para la gestión de huertos domésticos.

## Descripción

Hustle Garden es una aplicación móvil multiplataforma que permite a los usuarios:

- ?? Gestionar plantas de su huerto
- ?? Registrar riegos y mantener un historial
- ?? Documentar cosechas con fotos y calidad
- ? Administrar tareas pendientes
- ?? Crear notas categorizadas sobre el huerto
- ?? Visualizar estadísticas de productividad

## Tecnologías Utilizadas

- **.NET 10** - Framework principal
- **MAUI** - Framework multiplataforma para UI
- **Entity Framework Core** - ORM para base de datos
- **SQLite** - Base de datos local
- **PropertyChanged.Fody** - Implementación automática de INotifyPropertyChanged

## Estructura del Proyecto

### Modelos
- `Planta` - Representa una planta del huerto
- `Riego` - Registro de riegos
- `Cosecha` - Registro de cosechas
- `Tarea` - Tareas pendientes
- `NotaHuerto` - Notas y observaciones

### ViewModels
- `HuertoViewModel` - Gestión principal de plantas
- `DetallePlantaViewModel` - Detalle y operaciones de planta
- `TareasViewModel` - Gestión de tareas
- `NotasViewModel` - Gestión de notas
- `EstadisticasViewModel` - Visualización de estadísticas

### Servicios
- `ValidationService` - Validación de datos de entrada
- `ImageService` - Manejo de imágenes (captura y selección)

## Navegación

- [API Reference](api/index.md) - Documentación completa de la API
- [Modelos](api/HuertoApp.Models.html) - Clases de modelo
- [ViewModels](api/HuertoApp.ViewModels.html) - ViewModels MVVM
- [Servicios](api/HuertoApp.Services.html) - Servicios de la aplicación

## Inicio Rápido

1. Clone el repositorio
2. Abra la solución en Visual Studio 2022
3. Seleccione la plataforma de destino (Android, iOS, Windows)
4. Ejecute la aplicación

## Licencia

Este proyecto es de código abierto.
