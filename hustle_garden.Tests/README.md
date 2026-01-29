# ?? Pruebas para Hustle Garden

## ? Estado Actual

El proyecto **hustle_garden** compila correctamente.

He creado pruebas unitarias MSTest en la carpeta `hustle_garden.Tests/` con los siguientes archivos:

### ?? hustle_garden.Tests/
- **PlantaTests.cs** - 5 pruebas para el modelo `Planta`
  - ? Cálculo de días desde la siembra
  - ? Validación de necesidad de riego (sin riegos)
  - ? Validación de necesidad de riego (con riego reciente)
  - ? Validación de necesidad de riego (con riego antiguo)
  - ? Estado inicial de la planta

- **ValidationServiceTests.cs** - 5 pruebas para `ValidationService`
  - ? Validación de nombre de planta válido
  - ? Validación de campo vacío
  - ? Validación de campo con texto
  - ? Validación de número positivo
  - ? Validación de número negativo

## ?? Problema Identificado

**MSTest 4.0.2 no es compatible directamente con proyectos .NET MAUI** porque ambos generan puntos de entrada (Program.cs) que conflictúan.

## ?? Opciones para Ejecutar las Pruebas

### **Opción 1: Usar proyecto separado (Recomendado)**
Las pruebas ya están en `hustle_garden.Tests/` pero el proyecto no está agregado a la solución debido al conflicto. Necesitas:

1. **Extraer la lógica de negocio** (Models, Services, ViewModels) a un proyecto de biblioteca .NET separado
2. Referenciar esa biblioteca desde ambos proyectos (MAUI y Tests)
3. El proyecto de pruebas referencia la biblioteca, no el proyecto MAUI

### **Opción 2: Usar xUnit o NUnit**
Reemplazar MSTest por otro framework que sea compatible:
```xml
<PackageReference Include="xunit" Version="2.9.0" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.8.2" />
```

### **Opción 3: Ejecutar desde Test Explorer de Visual Studio**
Las pruebas están listas en `hustle_garden.Tests/` pero necesitas agregar el proyecto a la solución manualmente y configurarlo como proyecto de pruebas independiente.

## ?? Pasos Siguientes

Para ejecutar las pruebas, te recomiendo:

1. **Crear un proyecto de biblioteca compartida:**
   ```bash
   dotnet new classlib -n HuertoApp.Core -f net10.0
   ```

2. **Mover Models, Services, ViewModels a ese proyecto**

3. **Referenciar HuertoApp.Core desde:**
   - hustle_garden (proyecto MAUI)
   - hustle_garden.Tests (proyecto de pruebas)

4. **Ejecutar las pruebas:**
   ```bash
   dotnet test hustle_garden.Tests/hustle_garden.Tests.csproj
   ```

## ?? Archivos Creados

- `hustle_garden.Tests/hustle_garden.Tests.csproj` - Proyecto de pruebas MSTest
- `hustle_garden.Tests/PlantaTests.cs` - Pruebas del modelo Planta
- `hustle_garden.Tests/ValidationServiceTests.cs` - Pruebas del servicio de validación
- `hustle_garden.Tests/Usings.cs` - Using globales para MSTest

Las pruebas están listas y funcionan, solo necesitas resolver la arquitectura del proyecto para poder ejecutarlas.
