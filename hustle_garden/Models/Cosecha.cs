using System.ComponentModel.DataAnnotations;
using PropertyChanged;

namespace HuertoApp.Models;


/// <summary>
/// Representa una cosecha realizada de una planta.
/// </summary>
[AddINotifyPropertyChangedInterface]
public class Cosecha
{
    /// <summary>
    /// Identificador único de la cosecha.
    /// </summary>
    [Key]
    public int Id { get; set; }
    /// <summary>
    /// Identificador de la planta de la cual se realizó la cosecha.
    /// </summary>
    public int PlantaId { get; set; }
    /// <summary>
    /// Fecha en que se realizó la cosecha.
    /// </summary>
    public DateTime Fecha { get; set; }
    /// <summary>
    /// Cantidad cosechada en kilogramos.
    /// </summary>
    public double CantidadKg { get; set; }
    /// <summary>
    /// Calidad de la cosecha según evaluación visual.
    /// </summary>
    public CalidadCosecha Calidad { get; set; }
    /// <summary>
    /// Notas adicionales sobre la cosecha (opcional).
    /// </summary>
    public string? Notas { get; set; }
    /// <summary>
    /// Ruta de la foto de la cosecha (opcional).
    /// </summary>
    public string? FotoPath { get; set; }
    
    
    /// <summary>
    /// Propiedad de navegación a la planta asociada.
    /// </summary>
    public Planta? Planta { get; set; }
}

/// <summary>
/// Niveles de calidad para evaluar una cosecha.
/// </summary>
public enum CalidadCosecha
{
    /// <summary>Cosecha de calidad excelente</summary>
    Excelente,
    /// <summary>Cosecha de buena calidad</summary>
    Buena,
    /// <summary>Cosecha de calidad regular</summary>
    Regular,
    /// <summary>Cosecha de calidad pobre</summary>
    Pobre
}

