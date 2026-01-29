using HuertoApp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace hustle_garden.Tests;

[TestClass]
public class PlantaTests
{
    [TestMethod]
    public void Planta_DiasDesdeSeleccion_CalculaCorrectamente()
    {
        // Arrange
        var planta = new Planta
        {
            Nombre = "Tomate",
            FechaSiembra = DateTime.Now.AddDays(-10)
        };

        // Act
        int dias = planta.DiasDesdeSelección;

        // Assert
        Assert.AreEqual(10, dias);
    }

    [TestMethod]
    public void Planta_NecesitaRiego_SinRiegos_DevuelveTrue()
    {
        // Arrange
        var planta = new Planta
        {
            Nombre = "Lechuga",
            Riegos = new List<Riego>()
        };

        // Act
        bool necesitaRiego = planta.NecesitaRiego;

        // Assert
        Assert.IsTrue(necesitaRiego);
    }

    [TestMethod]
    public void Planta_NecesitaRiego_ConRiegoReciente_DevuelveFalse()
    {
        // Arrange
        var planta = new Planta
        {
            Nombre = "Zanahoria",
            Riegos = new List<Riego>
            {
                new Riego { Fecha = DateTime.Now.AddDays(-1), CantidadLitros = 2.0 }
            }
        };

        // Act
        bool necesitaRiego = planta.NecesitaRiego;

        // Assert
        Assert.IsFalse(necesitaRiego);
    }

    [TestMethod]
    public void Planta_NecesitaRiego_ConRiegoAntiguo_DevuelveTrue()
    {
        // Arrange
        var planta = new Planta
        {
            Nombre = "Pepino",
            Riegos = new List<Riego>
            {
                new Riego { Fecha = DateTime.Now.AddDays(-5), CantidadLitros = 3.0 }
            }
        };

        // Act
        bool necesitaRiego = planta.NecesitaRiego;

        // Assert
        Assert.IsTrue(necesitaRiego);
    }

    [TestMethod]
    public void Planta_EstadoInicial_EsGerminando()
    {
        // Arrange & Act
        var planta = new Planta
        {
            Nombre = "Berenjena"
        };

        // Assert
        Assert.AreEqual(EstadoPlanta.Germinando, planta.Estado);
    }
}
