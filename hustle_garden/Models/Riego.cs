using System.ComponentModel.DataAnnotations;
using PropertyChanged;

namespace HuertoApp.Models;

[AddINotifyPropertyChangedInterface]
public class Riego
{
    [Key]
    public int Id { get; set; }
    public int PlantaId { get; set; }
    public DateTime Fecha { get; set; }
    public double CantidadLitros { get; set; }
    public string? Notas { get; set; }
    
    // Navegación
    public Planta Planta { get; set; }
}
