using System.Globalization;
using HuertoApp.Models;

namespace HuertoApp.Converters;

/// <summary>
/// Convierte una categoría de nota a un emoji representativo.
/// </summary>
public class CategoriaToEmojiConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is CategoriaNota categoria)
        {
            return categoria switch
            {
                CategoriaNota.General => "🏷️",
                CategoriaNota.Clima => "🌤️",
                CategoriaNota.Plagas => "🐛",
                CategoriaNota.Fertilizacion => "🌱",
                CategoriaNota.Observacion => "👁️",
                CategoriaNota.Recordatorio => "⏰",
                _ => "🏷️"
            };
        }
        return "🏷️";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Convierte una categoría de nota a su color primario asociado.
/// </summary>
public class CategoriaToPrimaryColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is CategoriaNota categoria)
        {
            return categoria switch
            {
                CategoriaNota.General => Color.FromArgb("#8E24AA"),
                CategoriaNota.Clima => Color.FromArgb("#42A5F5"),
                CategoriaNota.Plagas => Color.FromArgb("#EF5350"),
                CategoriaNota.Fertilizacion => Color.FromArgb("#66BB6A"),
                CategoriaNota.Observacion => Color.FromArgb("#FFA726"),
                CategoriaNota.Recordatorio => Color.FromArgb("#26C6DA"),
                _ => Color.FromArgb("#8E24AA")
            };
        }
        return Color.FromArgb("#8E24AA");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Convierte una categoría de nota a su color secundario asociado.
/// </summary>
public class CategoriaToSecondaryColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is CategoriaNota categoria)
        {
            return categoria switch
            {
                CategoriaNota.General => Color.FromArgb("#AB47BC"),
                CategoriaNota.Clima => Color.FromArgb("#64B5F6"),
                CategoriaNota.Plagas => Color.FromArgb("#E57373"),
                CategoriaNota.Fertilizacion => Color.FromArgb("#81C784"),
                CategoriaNota.Observacion => Color.FromArgb("#FFB74D"),
                CategoriaNota.Recordatorio => Color.FromArgb("#4DD0E1"),
                _ => Color.FromArgb("#AB47BC")
            };
        }
        return Color.FromArgb("#AB47BC");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}