using System.ComponentModel.DataAnnotations;
using PropertyChanged;

namespace HuertoApp.Models;

/// <summary>
/// Representa una tarea o actividad pendiente relacionada con el huerto.
/// </summary>
[AddINotifyPropertyChangedInterface]
public class Tarea
{
    /// <summary>
    /// Identificador único de la tarea.
    /// </summary>
    [Key]
    public int Id { get; set; }
    /// <summary>
    /// Identificador de la planta asociada (opcional si la tarea es general).
    /// </summary>
    public int? PlantaId { get; set; }
    /// <summary>
    /// Título de la tarea.
    /// </summary>
    public string Titulo { get; set; } = string.Empty;
    /// <summary>
    /// Descripción detallada de la tarea (opcional).
    /// </summary>
    public string? Descripcion { get; set; }
    /// <summary>
    /// Fecha de creación de la tarea.
    /// </summary>
    public DateTime FechaCreacion { get; set; }
    /// <summary>
    /// Fecha límite para completar la tarea (opcional).
    /// </summary>
    public DateTime? FechaVencimiento { get; set; }
    /// <summary>
    /// Indica si la tarea ha sido completada.
    /// </summary>
    public bool Completada { get; set; }
    /// <summary>
    /// Nivel de prioridad de la tarea.
    /// </summary>
    public PrioridadTarea Prioridad { get; set; } = PrioridadTarea.Media;
    
    
    /// <summary>
    /// Propiedad de navegación a la planta asociada (si aplica).
    /// </summary>
    public Planta? Planta { get; set; }
    
    
    /// <summary>
    /// Indica si la tarea está vencida (tiene fecha de vencimiento pasada y no está completada).
    /// </summary>
    public bool EstaVencida => FechaVencimiento.HasValue && FechaVencimiento.Value < DateTime.Now && !Completada;
}

/// <summary>
/// Niveles de prioridad para las tareas.
/// </summary>
public enum PrioridadTarea
{
    /// <summary>Prioridad baja</summary>
    Baja,
    /// <summary>Prioridad media</summary>
    Media,
    /// <summary>Prioridad alta</summary>
    Alta,
    /// <summary>Prioridad urgente</summary>
    Urgente
}

