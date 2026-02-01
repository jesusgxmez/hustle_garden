# 🌱 Hustle Garden - Aplicación de Gestión de Huerto

<div align="center">

![.NET MAUI](https://img.shields.io/badge/.NET%20MAUI-10.0-512BD4?style=for-the-badge&logo=.net)
![Entity Framework](https://img.shields.io/badge/EF%20Core-10.0-512BD4?style=for-the-badge&logo=.net)
![SQLite](https://img.shields.io/badge/SQLite-3.0-003B57?style=for-the-badge&logo=sqlite)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

Una aplicación multiplataforma para gestionar tu huerto doméstico de forma completa y sencilla.

[Características](#-características) • [Instalación](#-instalación) • [Documentación](#-documentación) • [Contribuir](#-contribuir)

</div>

---

## 📱 Acerca del Proyecto

**Hustle Garden** es una aplicación móvil desarrollada con .NET MAUI que te permite llevar un control completo de tu huerto o jardín. Registra tus plantas, gestiona riegos y cosechas, crea tareas pendientes, toma notas y visualiza estadísticas de productividad, todo en una interfaz intuitiva y moderna.

### 🎯 ¿Para quién es?

- 🏡 Horticultores domésticos que quieren organizar su huerto
- 🌾 Personas que cultivan sus propios alimentos
- 🌿 Aficionados a la jardinería que desean hacer seguimiento de sus plantas
- 👨‍🌾 Cualquiera que quiera aprender y mejorar sus cultivos

---

## ✨ Características

### 🌱 Gestión de Plantas
- ➕ **Agregar plantas** con nombre, variedad, ubicación y foto
- 📸 **Captura de fotos** desde la cámara o galería
- 📅 **Seguimiento del ciclo de vida** con estados: Germinando, Creciendo, Floreciendo, Fructificando, Lista para Cosechar, Cosechada, Marchita
- 🔍 **Vista detallada** de cada planta con toda su información
- 🗑️ **Eliminar plantas** con confirmación de seguridad

### 💧 Sistema de Riego
- 📝 **Registrar riegos** con cantidad en litros y fecha
- 📊 **Historial de riegos** por planta
- ⚠️ **Alertas automáticas** de plantas que necesitan riego (más de 2 días sin regar)
- 📈 **Seguimiento del consumo de agua**

### 🌾 Registro de Cosechas
- 🎯 **Registrar cosechas** con cantidad en kg
- ⭐ **Calidad de cosecha**: Excelente, Buena, Regular o Pobre
- 📸 **Fotos de cosechas** para documentar resultados
- 📝 **Notas adicionales** sobre cada cosecha
- 🔄 **Cambio automático** de estado de la planta a "Cosechada"

### ✅ Gestión de Tareas
- 📋 **Crear tareas** del huerto
- 🔗 **Asociar tareas** a plantas específicas o tareas generales
- 🎚️ **Prioridades**: Baja, Media, Alta, Urgente
- ✔️ **Marcar como completadas** con checkbox
- 📅 **Fechas de vencimiento** con indicador de vencidas
- 👆 **Swipe para eliminar** tareas

### 📝 Notas y Diario
- 📄 **Crear notas** con título, contenido y foto
- 🏷️ **Categorías**: General, Clima, Plagas, Fertilización, Observación, Recordatorio
- 📅 **Ordenadas por fecha** para llevar un diario del huerto
- 🖼️ **Adjuntar fotos** a cada nota
- 🗑️ **Eliminar con swipe**

### 📊 Estadísticas y Análisis
- 📈 **Dashboard completo** con métricas clave:
  - Total de plantas y plantas activas
  - Total de cosechas y kg cosechados
  - Plantas que necesitan riego
  - Tareas pendientes
- 📅 **Cosechas por mes** (últimos 6 meses)
- 🏆 **Top 5 plantas más productivas**
- 🔄 **Actualización en tiempo real**

---

## 🛠️ Tecnologías

- **Framework:** .NET 10 con .NET MAUI
- **Base de datos:** SQLite con Entity Framework Core
- **Patrón:** MVVM (Model-View-ViewModel)
- **Librerías:**
  - PropertyChanged.Fody (INotifyPropertyChanged automático)
  - Microsoft.EntityFrameworkCore.Sqlite
  - MediaPicker para fotos
- **Plataformas:** Android, iOS, Windows, macOS

---

## 📥 Instalación

### Requisitos Previos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) o [Visual Studio Code](https://code.visualstudio.com/)
- Para desarrollo móvil:
  - **Android:** Android SDK 21+
  - **iOS:** Xcode 15+ (solo en macOS)
  - **Windows:** Windows 10 SDK 19041+

### Clonar el Repositorio

```bash
git clone https://github.com/jesusgxmez/hustle_garden.git
cd hustle_garden
```

### Restaurar Dependencias

```bash
dotnet restore
```

### Compilar el Proyecto

```bash
dotnet build
```

### Ejecutar la Aplicación

#### Android
```bash
dotnet build -t:Run -f net10.0-android
```

#### iOS (requiere macOS)
```bash
dotnet build -t:Run -f net10.0-ios
```

#### Windows
```bash
dotnet build -t:Run -f net10.0-windows10.0.19041.0
```

### Visual Studio

1. Abrir `hustle_garden.sln`
2. Seleccionar la plataforma de destino en la barra de herramientas
3. Presionar F5 o hacer clic en "Ejecutar"

---

## 📖 Documentación

### Documentación del Código (DocFX)

El proyecto incluye documentación completa generada con DocFX. Para visualizarla:

#### Opción 1: Script Automatizado (Recomendado)

```powershell
cd Documentacion
.\GenerarDocumentacion.ps1 -Servir
```

Esto generará y abrirá automáticamente la documentación en http://localhost:8080

#### Opción 2: Comandos Manuales

```bash
# Navegar a la carpeta de documentación
cd Documentacion

# Generar la documentación
docfx docfx.json

# Servir localmente
docfx serve _site
```

Luego abrir en el navegador: http://localhost:8080

#### Requisitos para DocFX

Si DocFX no está instalado:

```bash
dotnet tool install -g docfx
```

### Archivos de Documentación

- 📄 **[DOCUMENTACION.md](hustle_garden/DOCUMENTACION.md)** - Documentación principal de la aplicación
- 📄 **[DOCUMENTACION_RESUMEN.md](DOCUMENTACION_RESUMEN.md)** - Resumen de documentación XML y DocFX
- 📄 **[DOCUMENTACION_INICIO_RAPIDO.md](DOCUMENTACION_INICIO_RAPIDO.md)** - Guía rápida
- 📄 **[CAMBIOS_CONTRASTE.md](CAMBIOS_CONTRASTE.md)** - Mejoras de contraste visual
- 📁 **[Documentacion/](Documentacion/)** - Carpeta completa de DocFX

---

## 🏗️ Estructura del Proyecto

```
hustle_garden/
├── hustle_garden/                 # Proyecto principal .NET MAUI
│   ├── Models/                    # Modelos de datos
│   ├── ViewModels/               # ViewModels (MVVM)
│   ├── Views/                    # Páginas XAML
│   ├── Data/                     # DbContext y configuración de BD
│   ├── Services/                 # Servicios (validación, imágenes)
│   ├── Converters/               # Convertidores de valores
│   ├── Resources/                # Recursos (imágenes, fuentes)
│   └── Platforms/                # Código específico de plataforma
├── HuertoApp.Core/               # Biblioteca compartida
├── hustle_garden.Tests/          # Tests unitarios
├── Documentacion/                # Documentación DocFX
└── README.md                     # Este archivo
```

---

## 🚀 Características Destacadas

### 🎯 Patrón MVVM con Fody
- Separación clara de responsabilidades
- PropertyChanged.Fody para notificaciones automáticas
- Binding bidireccional completo

### 💾 Base de Datos Local
- SQLite para almacenamiento local
- Entity Framework Core como ORM
- Relaciones configuradas con Fluent API
- Migraciones automáticas

### 🔄 Inyección de Dependencias
- ViewModels y servicios registrados en el contenedor DI
- Lifetime management adecuado
- Constructor injection en páginas y ViewModels

### 📱 Interfaz Moderna
- Diseño con tema verde (huerto)
- Cards con sombras y bordes redondeados
- Gradientes y efectos visuales
- SwipeView para acciones rápidas
- Gestos táctiles intuitivos

---

## 🔧 Configuración Inicial

### Primera Ejecución

Después de ejecutar la aplicación por **primera vez**:

1. Abrir `hustle_garden/Data/HuertoContext.cs`
2. Comentar la línea `this.Database.EnsureDeleted();`

```csharp
public HuertoContext()
{
    SQLitePCL.Batteries_V2.Init();
    
    // COMENTAR después de la primera ejecución:
    // this.Database.EnsureDeleted();
    
    this.Database.EnsureCreated();
}
```

Esto evitará que se elimine la base de datos en cada ejecución.

---

## 🧪 Testing

Ejecutar los tests unitarios:

```bash
dotnet test
```

---


### 🎯 Versión Actual (v1.0)
- ✅ CRUD completo de plantas
- ✅ Sistema de riego
- ✅ Registro de cosechas
- ✅ Gestión de tareas
- ✅ Notas con categorías
- ✅ Estadísticas básicas
- ✅ Documentación completa


---


## 👥 Autores

* **Leo Murillo** - [@leomggg](https://github.com/leomggg)
* **Jesús Gómez** - [@jesusgxmez](https://github.com/jesusgxmez)
* **Daniel Morales** - [@danielzetazz](https://github.com/danielzetazz)
* **Iván Tejero** - [@tjerito](https://github.com/tjerito)


---

## 📊 Estado del Proyecto

| Aspecto | Estado |
|---------|--------|
| Compilación | ✅ Exitosa |
| Tests | ✅ Pasando |
| Documentación | ✅ Completa |
| Patrón MVVM | ✅ Implementado |
| Base de Datos | ✅ Funcional |
| UI/UX | ✅ Diseñada |
| Multiplataforma | ✅ Soportado |

---


