using HuertoApp.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace hustle_garden.Tests;

[TestClass]
public class ValidationServiceTests
{
    private ValidationService _validationService;

    [TestInitialize]
    public void Setup()
    {
        _validationService = new ValidationService();
    }

    [TestMethod]
    public void ValidatePlantName_NombreValido_DevuelveSuccess()
    {
        // Arrange
        string nombreValido = "Tomate Cherry";

        // Act
        var result = _validationService.ValidatePlantName(nombreValido);

        // Assert
        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public void ValidateNotEmpty_ValorVacio_DevuelveError()
    {
        // Arrange
        string valorVacio = "";

        // Act
        var result = _validationService.ValidateNotEmpty(valorVacio, "Campo");

        // Assert
        Assert.IsFalse(result.IsValid);
    }

    [TestMethod]
    public void ValidateNotEmpty_ValorConTexto_DevuelveSuccess()
    {
        // Arrange
        string valorConTexto = "Algo de texto";

        // Act
        var result = _validationService.ValidateNotEmpty(valorConTexto, "Campo");

        // Assert
        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public void ValidatePositiveNumber_NumeroPositivo_DevuelveSuccess()
    {
        // Arrange
        double numeroPositivo = 5.5;

        // Act
        var result = _validationService.ValidatePositiveNumber(numeroPositivo, "Cantidad");

        // Assert
        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public void ValidatePositiveNumber_NumeroNegativo_DevuelveError()
    {
        // Arrange
        double numeroNegativo = -1.0;

        // Act
        var result = _validationService.ValidatePositiveNumber(numeroNegativo, "Cantidad");

        // Assert
        Assert.IsFalse(result.IsValid);
    }
}
