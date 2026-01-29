using System.Globalization;

namespace HuertoApp.Converters;

/// <summary>
/// Convierte una cadena de texto a un booleano indicando si no es nula o vacía.
/// </summary>
public class StringNotNullOrEmptyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return !string.IsNullOrWhiteSpace(value as string);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Convierte un objeto a booleano indicando si no es nulo.
/// Soporta inversión del resultado mediante parámetro "Invert".
/// </summary>
public class IsNotNullConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isNotNull = value != null;
        
        if (parameter is string param && param.Equals("Invert", StringComparison.OrdinalIgnoreCase))
        {
            return !isNotNull;
        }
        
        return isNotNull;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
