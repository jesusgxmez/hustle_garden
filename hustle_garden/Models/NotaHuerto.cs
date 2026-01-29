using System.ComponentModel.DataAnnotations;
using PropertyChanged;

namespace HuertoApp.Models;

/// <summary>
/// Representa una nota o apunte relacionado con el huerto.
/// </summary>
[AddINotifyPropertyChangedInterface]
public class NotaHuerto
{
    /// <summary>
    /// Identificador único de la nota.
    /// </summary>
    [Key]
    public int Id { get; set; }
    /// <summary>
    /// Título de la nota.
    /// </summary>
    public string Titulo { get; set; } = string.Empty;
    /// <summary>
    /// Contenido o cuerpo de la nota.
    /// </summary>
    public string Contenido { get; set; } = string.Empty;
    /// <summary>
    /// Fecha de creación de la nota.
    /// </summary>
    public DateTime Fecha { get; set; }
    /// <summary>
    /// Categoría de la nota para clasificación.
    /// </summary>
    public CategoriaNota Categoria { get; set; } = CategoriaNota.General;
    /// <summary>
    /// Ruta de la foto asociada a la nota (opcional).
    /// </summary>
    public string? FotoPath { get; set; }
}

/// <summary>
/// Categorías disponibles para clasificar notas del huerto.
/// </summary>
public enum CategoriaNota
{
    /// <summary>Nota general sin categoría específica</summary>
    General,
    /// <summary>Nota relacionada con el clima</summary>
    Clima,
    /// <summary>Nota sobre plagas o problemas fitosanitarios</summary>
    Plagas,
    /// <summary>Nota sobre fertilización o abonado</summary>
    Fertilizacion,
    /// <summary>Observación general del huerto</summary>
    Observacion,
    /// <summary>Recordatorio de tareas pendientes</summary>
    Recordatorio
}
