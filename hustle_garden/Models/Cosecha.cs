using System.ComponentModel.DataAnnotations;
using PropertyChanged;

namespace HuertoApp.Models;


[AddINotifyPropertyChangedInterface]
public class Cosecha
{
    [Key]
    public int Id { get; set; }
    public int PlantaId { get; set; }
    public DateTime Fecha { get; set; }
    public double CantidadKg { get; set; }
    public CalidadCosecha Calidad { get; set; }
    public string? Notas { get; set; }
    public string? FotoPath { get; set; }
    
    public Planta? Planta { get; set; }
}

public enum CalidadCosecha
{
    Excelente,
    Buena,
    Regular,
    Pobre
}

