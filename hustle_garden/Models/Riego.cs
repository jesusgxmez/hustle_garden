using System.ComponentModel.DataAnnotations;
using PropertyChanged;

namespace HuertoApp.Models;

/// <summary>
/// Representa un registro de riego para una planta.
/// </summary>
[AddINotifyPropertyChangedInterface]
public class Riego
{
    /// <summary>
    /// Identificador único del registro de riego.
    /// </summary>
    [Key]
    public int Id { get; set; }
    /// <summary>
    /// Identificador de la planta asociada a este riego.
    /// </summary>
    public int PlantaId { get; set; }
    /// <summary>
    /// Fecha y hora en que se realizó el riego.
    /// </summary>
    public DateTime Fecha { get; set; }
    /// <summary>
    /// Cantidad de agua utilizada en el riego, expresada en litros.
    /// </summary>
    public double CantidadLitros { get; set; }
    /// <summary>
    /// Notas adicionales sobre el riego (opcional).
    /// </summary>
    public string? Notas { get; set; }
    
    
    /// <summary>
    /// Propiedad de navegación a la planta asociada.
    /// </summary>
    public Planta? Planta { get; set; }
}

