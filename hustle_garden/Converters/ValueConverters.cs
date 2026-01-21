using System.Globalization;

namespace HuertoApp.Converters;

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

public class IsNotNullConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isNotNull = value != null;
        
        // Si el parámetro es "Invert", invertir el resultado
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
