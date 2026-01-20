using System.ComponentModel.DataAnnotations;
using PropertyChanged;

namespace HuertoApp.Models;

[AddINotifyPropertyChangedInterface]
public class Tarea
{
    [Key]
    public int Id { get; set; }
    public int? PlantaId { get; set; } // Puede ser null para tareas generales del huerto
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public bool Completada { get; set; }
    public PrioridadTarea Prioridad { get; set; } = PrioridadTarea.Media;
    
    // Navegación
    public Planta Planta { get; set; }
    
    // Propiedades calculadas
    public bool EstaVencida => FechaVencimiento.HasValue && FechaVencimiento.Value < DateTime.Now && !Completada;
}

public enum PrioridadTarea
{
    Baja,
    Media,
    Alta,
    Urgente
}
