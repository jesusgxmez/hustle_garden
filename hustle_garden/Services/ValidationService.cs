using System.Text.RegularExpressions;

namespace HuertoApp.Services;

/// <summary>
/// Interfaz para servicios de validación de datos de entrada.
/// </summary>
public interface IValidationService
{
    /// <summary>
    /// Valida el nombre de una planta.
    /// </summary>
    /// <param name="nombre">Nombre de la planta a validar.</param>
    /// <returns>Resultado de la validación.</returns>
    ValidationResult ValidatePlantName(string nombre);
    /// <summary>
    /// Valida que un número esté dentro de un rango especificado.
    /// </summary>
    /// <param name="value">Valor a validar.</param>
    /// <param name="min">Valor mínimo permitido.</param>
    /// <param name="max">Valor máximo permitido.</param>
    /// <param name="fieldName">Nombre del campo para mensajes de error.</param>
    /// <returns>Resultado de la validación.</returns>
    ValidationResult ValidateNumberInRange(double value, double min, double max, string fieldName);
    /// <summary>
    /// Valida que un número sea positivo y válido.
    /// </summary>
    /// <param name="value">Valor a validar.</param>
    /// <param name="fieldName">Nombre del campo para mensajes de error.</param>
    /// <returns>Resultado de la validación.</returns>
    ValidationResult ValidatePositiveNumber(double value, string fieldName);
    /// <summary>
    /// Valida que una cadena de texto no esté vacía.
    /// </summary>
    /// <param name="value">Cadena a validar.</param>
    /// <param name="fieldName">Nombre del campo para mensajes de error.</param>
    /// <returns>Resultado de la validación.</returns>
    ValidationResult ValidateNotEmpty(string value, string fieldName);
    /// <summary>
    /// Valida que una fecha esté dentro de un rango especificado.
    /// </summary>
    /// <param name="date">Fecha a validar.</param>
    /// <param name="minDate">Fecha mínima permitida (opcional).</param>
    /// <param name="maxDate">Fecha máxima permitida (opcional).</param>
    /// <param name="fieldName">Nombre del campo para mensajes de error.</param>
    /// <returns>Resultado de la validación.</returns>
    ValidationResult ValidateDateRange(DateTime date, DateTime? minDate, DateTime? maxDate, string fieldName);
    /// <summary>
    /// Valida la ruta de una imagen (existencia, formato y tamaño).
    /// </summary>
    /// <param name="path">Ruta de la imagen a validar.</param>
    /// <returns>Resultado de la validación.</returns>
    ValidationResult ValidateImagePath(string path);
}

/// <summary>
/// Implementación del servicio de validación de datos de entrada.
/// </summary>
public class ValidationService : IValidationService
{
    private const int MAX_PLANT_NAME_LENGTH = 100;
    private const int MIN_PLANT_NAME_LENGTH = 2;
    private const double MAX_DAYS_TO_HARVEST = 365;
    private const double MAX_WATER_LITERS = 1000;
    private const double MAX_HARVEST_KG = 10000;
    private const double MIN_WATER_LITERS = 0.1; 
    private const double MIN_HARVEST_KG = 0.01; 

    public ValidationResult ValidatePlantName(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return ValidationResult.Failure("El nombre de la planta es obligatorio");
        }

        if (nombre.Length < MIN_PLANT_NAME_LENGTH)
        {
            return ValidationResult.Failure($"El nombre debe tener al menos {MIN_PLANT_NAME_LENGTH} caracteres");
        }

        if (nombre.Length > MAX_PLANT_NAME_LENGTH)
        {
            return ValidationResult.Failure($"El nombre no puede exceder {MAX_PLANT_NAME_LENGTH} caracteres");
        }

        if (!Regex.IsMatch(nombre, @"[a-zA-ZáéíóúÁÉÍÓÚñÑ]"))
        {
            return ValidationResult.Failure("El nombre debe contener al menos una letra");
        }

        return ValidationResult.Success();
    }

    public ValidationResult ValidateNumberInRange(double value, double min, double max, string fieldName)
    {
        if (value < min || value > max)
        {
            return ValidationResult.Failure($"{fieldName} debe estar entre {min} y {max}");
        }

        return ValidationResult.Success();
    }

    public ValidationResult ValidatePositiveNumber(double value, string fieldName)
    {
        if (value < 0)
        {
            return ValidationResult.Failure($"{fieldName} no puede ser negativo");
        }

        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            return ValidationResult.Failure($"{fieldName} no es un número válido");
        }

        return ValidationResult.Success();
    }

    public ValidationResult ValidateNotEmpty(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return ValidationResult.Failure($"{fieldName} es obligatorio");
        }

        return ValidationResult.Success();
    }

    public ValidationResult ValidateDateRange(DateTime date, DateTime? minDate, DateTime? maxDate, string fieldName)
    {
        if (minDate.HasValue && date < minDate.Value)
        {
            return ValidationResult.Failure($"{fieldName} no puede ser anterior a {minDate.Value:dd/MM/yyyy}");
        }

        if (maxDate.HasValue && date > maxDate.Value)
        {
            return ValidationResult.Failure($"{fieldName} no puede ser posterior a {maxDate.Value:dd/MM/yyyy}");
        }

        return ValidationResult.Success();
    }

    public ValidationResult ValidateImagePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return ValidationResult.Success(); 
        }

        if (!File.Exists(path))
        {
            return ValidationResult.Failure("La imagen seleccionada no existe");
        }

        var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
        var extension = Path.GetExtension(path).ToLowerInvariant();

        if (!validExtensions.Contains(extension))
        {
            return ValidationResult.Failure("Formato de imagen no válido. Use JPG, PNG, GIF o BMP");
        }

        var fileInfo = new FileInfo(path);
        if (fileInfo.Length > 10 * 1024 * 1024)
        {
            return ValidationResult.Failure("La imagen no puede superar 10MB");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Valida el número de días hasta la cosecha.
    /// </summary>
    /// <param name="days">Número de días a validar.</param>
    /// <returns>Resultado de la validación.</returns>
    public static ValidationResult ValidateDaysToHarvest(int days)
    {
        if (days < 0)
        {
            return ValidationResult.Failure("Los días hasta cosecha no pueden ser negativos");
        }

        if (days > MAX_DAYS_TO_HARVEST)
        {
            return ValidationResult.Failure($"Los días hasta cosecha no pueden exceder {MAX_DAYS_TO_HARVEST}");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Valida la cantidad de agua para un riego.
    /// </summary>
    /// <param name="liters">Cantidad de agua en litros a validar.</param>
    /// <returns>Resultado de la validación.</returns>
    public static ValidationResult ValidateWaterAmount(double liters)
    {
        if (liters <= 0)
        {
            return ValidationResult.Failure("La cantidad de agua debe ser mayor a 0");
        }

        if (liters < MIN_WATER_LITERS)
        {
            return ValidationResult.Failure($"La cantidad de agua debe ser al menos {MIN_WATER_LITERS} litros (100ml)");
        }

        if (liters > MAX_WATER_LITERS)
        {
            return ValidationResult.Failure($"La cantidad de agua no puede exceder {MAX_WATER_LITERS} litros");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Valida la cantidad cosechada.
    /// </summary>
    /// <param name="kg">Cantidad cosechada en kilogramos a validar.</param>
    /// <returns>Resultado de la validación.</returns>
    public static ValidationResult ValidateHarvestAmount(double kg)
    {
        if (kg <= 0)
        {
            return ValidationResult.Failure("La cantidad cosechada debe ser mayor a 0");
        }

        if (kg < MIN_HARVEST_KG)
        {
            return ValidationResult.Failure($"La cantidad cosechada debe ser al menos {MIN_HARVEST_KG} kg (10 gramos)");
        }

        if (kg > MAX_HARVEST_KG)
        {
            return ValidationResult.Failure($"La cantidad cosechada no puede exceder {MAX_HARVEST_KG} kg");
        }

        return ValidationResult.Success();
    }
}

/// <summary>
/// Representa el resultado de una operación de validación.
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Indica si la validación fue exitosa.
    /// </summary>
    public bool IsValid { get; private set; }
    /// <summary>
    /// Mensaje de error en caso de validación fallida.
    /// </summary>
    public string ErrorMessage { get; private set; }

    private ValidationResult(bool isValid, string errorMessage = "")
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Crea un resultado de validación exitosa.
    /// </summary>
    /// <returns>Resultado de validación exitosa.</returns>
    public static ValidationResult Success() => new ValidationResult(true);
    /// <summary>
    /// Crea un resultado de validación fallida con un mensaje de error.
    /// </summary>
    /// <param name="errorMessage">Mensaje de error que describe el problema.</param>
    /// <returns>Resultado de validación fallida.</returns>
    public static ValidationResult Failure(string errorMessage) => new ValidationResult(false, errorMessage);
}
