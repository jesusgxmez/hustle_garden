# DOCUMENTACIÓN - APLICACIÓN DE GESTIÓN DE HUERTO

## ?? CAMBIOS RECIENTES (IMPORTANTE)

### Cambio en Base de Datos - FotoPath Ahora es Opcional
Se ha modificado la estructura de la base de datos para permitir que las plantas se guarden sin foto.

**?? IMPORTANTE - ACCIÓN REQUERIDA:**
Después de ejecutar la aplicación por **PRIMERA VEZ** tras este cambio, debes:

1. Abrir el archivo `hustle_garden\Data\HuertoContext.cs`
2. Comentar la línea `this.Database.EnsureDeleted();` para evitar que se elimine la base de datos en cada ejecución
3. El código debería quedar así:

```csharp
public HuertoContext()
{
    SQLitePCL.Batteries_V2.Init();
    
    // COMENTAR después de la primera ejecución:
    // this.Database.EnsureDeleted();
    this.Database.EnsureCreated();
}
```

### Corrección de Colores en Campos de Texto
Se han agregado colores a todos los campos de entrada de texto (Entry y Editor) para mejorar la visibilidad:
- `TextColor="Black"` - El texto ingresado ahora se ve claramente en negro
- `PlaceholderColor="Gray"` - Los placeholders mantienen el color gris para diferenciarse

---

## ?? RESUMEN DE FUNCIONALIDADES IMPLEMENTADAS

### ? CARACTERÍSTICAS PRINCIPALES

#### 1. **GESTIÓN DE PLANTAS**
- ? Agregar nuevas plantas con información completa:
  - Nombre, variedad, ubicación
  - Foto desde cámara o galería
  - Días estimados hasta cosecha
  - Estado de la planta (Germinando, Creciendo, Floreciendo, etc.)
- ? Ver listado de todas las plantas
- ? Ver detalles completos de cada planta
- ? Cambiar estado de las plantas
- ? Eliminar plantas con confirmación
- ? Tap en planta para ver detalles completos

#### 2. **SISTEMA DE RIEGO**
- ? Registrar riegos con cantidad en litros
- ? Ver historial de riegos por planta
- ? Indicador de plantas que necesitan riego
- ? Cálculo automático basado en último riego (más de 2 días)

#### 3. **GESTIÓN DE COSECHAS**
- ? Registrar cosechas con:
  - Cantidad en kg
  - Calidad (Excelente, Buena, Regular, Pobre)
  - Notas opcionales
  - Foto de la cosecha
- ? Ver historial de cosechas por planta
- ? Cambio automático de estado a "Cosechada"

#### 4. **SISTEMA DE TAREAS**
- ? Crear tareas del huerto
- ? Asociar tareas a plantas específicas o tareas generales
- ? Asignar prioridad (Baja, Media, Alta, Urgente)
- ? Marcar tareas como completadas
- ? Ver tareas pendientes y completadas por separado
- ? Eliminar tareas con swipe
- ? Indicador de tareas vencidas

#### 5. **NOTAS Y DIARIO DEL HUERTO**
- ? Crear notas con título y contenido
- ? Categorizar notas:
  - General
  - Clima
  - Plagas
  - Fertilización
  - Observación
  - Recordatorio
- ? Adjuntar fotos a las notas
- ? Ver historial ordenado por fecha
- ? Eliminar notas con swipe

#### 6. **ESTADÍSTICAS Y ANÁLISIS**
- ? Panel de resumen con:
  - Total de plantas
  - Plantas activas
  - Total de cosechas
  - Kg totales cosechados
  - Plantas que necesitan riego
  - Tareas pendientes
- ? Cosechas por mes (últimos 6 meses)
- ? Top 5 plantas más productivas
- ? Botón de refresco manual de datos

---

## ??? ARQUITECTURA

### PATRÓN MVVM CON FODY
- ? Separación clara de responsabilidades
- ? PropertyChanged.Fody para INotifyPropertyChanged automático
- ? Binding bidireccional completo
- ? Commands para todas las acciones

### INYECCIÓN DE DEPENDENCIAS
- ? ViewModels registrados en DI container
- ? HuertoContext (EF Core) con lifetime adecuado
- ? Páginas con constructor injection

### BASE DE DATOS
- ? Entity Framework Core con SQLite
- ? Relaciones configuradas correctamente:
  - Planta ? Riegos (1:N)
  - Planta ? Tareas (1:N, DeleteBehavior.SetNull)
  - Planta ? Cosechas (1:N)
- ? Migraciones automáticas con EnsureCreated

---

## ?? ESTRUCTURA DE ARCHIVOS

### MODELOS (`Models/`)
- ? `Planta.cs` - Modelo principal con propiedades calculadas
- ? `Riego.cs` - Registro de riegos
- ? `Tarea.cs` - Sistema de tareas
- ? `Cosecha.cs` - Registro de cosechas
- ? `NotaHuerto.cs` - Notas y diario
- ? Enums: EstadoPlanta, PrioridadTarea, CalidadCosecha, CategoriaNota

### VIEWMODELS (`ViewModel/`)
- ? `HuertoViewModel.cs` - Gestión de plantas principal
- ? `DetallePlantaViewModel.cs` - Detalles y acciones de planta
- ? `TareasViewModel.cs` - Gestión de tareas
- ? `EstadisticasViewModel.cs` - Cálculos y estadísticas
- ? `NotasViewModel.cs` - Gestión de notas

### VISTAS (`Views/`)
- ? `MainPage.xaml` - Lista de plantas y agregar nueva
- ? `DetallePlantaPage.xaml` - Detalles, riegos y cosechas
- ? `TareasPage.xaml` - Gestión de tareas
- ? `EstadisticasPage.xaml` - Dashboard de estadísticas
- ? `NotasPage.xaml` - Notas del huerto

### DATA (`Data/`)
- ? `HuertoContext.cs` - DbContext con todas las entidades

### CONVERTERS (`Converters/`)
- ? `StringNotNullOrEmptyConverter` - Para visibilidad condicional
- ? `IsNotNullConverter` - Para visibilidad de objetos

---

## ?? CONFIGURACIÓN

### `MauiProgram.cs`
```csharp
- DbContext registrado
- Todos los ViewModels registrados como Transient
- Todas las páginas registradas
```

### `AppShell.xaml`
```xaml
- TabBar con 4 tabs:
  1. Plantas
  2. Tareas
  3. Notas
  4. Estadísticas
- Routing a DetallePlantaPage
```

### `App.xaml`
```xaml
- Converters registrados globalmente
- Estilos y colores heredados
```

---

## ?? CARACTERÍSTICAS DE UI/UX

### DISEÑO
- ? Esquema de colores verde temático (huerto)
- ? Frames con sombras y bordes redondeados
- ? Cards para cada elemento
- ? Iconos y emojis (en código, no en atributos XAML)
- ? Colores diferenciados por secciones

### INTERACCIONES
- ? SwipeView para eliminar (plantas, tareas, notas)
- ? TapGesture para navegar a detalles
- ? CheckBox para completar tareas
- ? Picker para selecciones
- ? DatePicker para fechas
- ? MediaPicker para fotos

### VALIDACIONES
- ? Validación de campos obligatorios
- ? Confirmaciones antes de eliminar
- ? Mensajes de éxito/error con DisplayAlert
- ? Visibilidad condicional con Converters

---

## ?? FUNCIONALIDADES AVANZADAS

### PROPIEDADES CALCULADAS
- `DiasDesdeSelección` - Días desde la siembra
- `NecesitaRiego` - Basado en último riego
- `EstaVencida` - Para tareas

### NAVEGACIÓN
- Shell con TabBar
- Navegación a páginas de detalle con parámetros
- QueryProperty para pasar IDs

### MULTIMEDIA
- Captura de fotos con cámara
- Selección de galería
- Almacenamiento en FileSystem.AppDataDirectory

---

## ?? DATOS Y PERSISTENCIA

### ALMACENAMIENTO
- SQLite local en el dispositivo
- Ruta: `FileSystem.AppDataDirectory/huerto_ef.db3`
- Entity Framework Core para ORM
- Relaciones configuradas con Fluent API

### OPERACIONES
- CRUD completo en todas las entidades
- Include para cargar relaciones
- OrderBy para ordenación
- Filtros y búsquedas

---

## ? MEJORAS IMPLEMENTADAS

1. **Formularios mejorados** - Más campos para información completa
2. **Historial completo** - Tracking de todas las acciones
3. **Estadísticas útiles** - KPIs y gráficos del huerto
4. **Sistema de tareas** - Para no olvidar labores del huerto
5. **Diario/Notas** - Documentar observaciones y aprendizajes
6. **Calidad de cosechas** - Para mejorar cultivos futuros
7. **Ubicación de plantas** - Organizar el espacio
8. **Estados de plantas** - Seguimiento del ciclo de vida
9. **Prioridades en tareas** - Gestión del tiempo
10. **Converters reutilizables** - Para binding limpio

---

## ?? POSIBLES EXTENSIONES FUTURAS

- [ ] Recordatorios con notificaciones locales
- [ ] Gráficos más avanzados (charts)
- [ ] Exportar datos (CSV, PDF)
- [ ] Sincronización en la nube
- [ ] Compartir fotos en redes sociales
- [ ] Calendario de siembra según clima
- [ ] Base de datos de plantas (wiki)
- [ ] Modo offline completo
- [ ] Geolocalización del huerto
- [ ] Predicciones con IA

---

## ??? TECNOLOGÍAS UTILIZADAS

- .NET MAUI (NET 10)
- Entity Framework Core
- SQLite
- PropertyChanged.Fody
- MVVM Pattern
- Dependency Injection
- Value Converters
- Shell Navigation
- MediaPicker
- FileSystem API

---

## ? ESTADO DEL PROYECTO

**COMPILACIÓN**: ? EXITOSA
**PATRÓN MVVM**: ? IMPLEMENTADO CORRECTAMENTE
**FODY**: ? CONFIGURADO
**BASE DE DATOS**: ? FUNCIONAL
**NAVEGACIÓN**: ? CONFIGURADA
**UI/UX**: ? DISEÑADA

**LISTO PARA**: Pruebas en dispositivo y mejoras iterativas
