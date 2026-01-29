using System.ComponentModel.DataAnnotations;
using PropertyChanged;

namespace HuertoApp.Models;

/// <summary>
/// Representa una planta en el huerto con su información de seguimiento.
/// </summary>
[AddINotifyPropertyChangedInterface]
public class Planta
{
    /// <summary>
    /// Identificador único de la planta.
    /// </summary>
    [Key]
    public int Id { get; set; }
    /// <summary>
    /// Nombre de la planta.
    /// </summary>
    public string Nombre { get; set; } = string.Empty;
    /// <summary>
    /// Variedad específica de la planta (opcional).
    /// </summary>
    public string? Variedad { get; set; }
    /// <summary>
    /// Fecha en la que se sembró la planta.
    /// </summary>
    public DateTime FechaSiembra { get; set; }
    /// <summary>
    /// Fecha estimada para la cosecha (opcional).
    /// </summary>
    public DateTime? FechaCosechaEstimada { get; set; }
    /// <summary>
    /// Ruta de la foto de la planta (opcional).
    /// </summary>
    public string? FotoPath { get; set; }
    /// <summary>
    /// Ubicación de la planta en el huerto (opcional).
    /// </summary>
    public string? Ubicacion { get; set; } 
    /// <summary>
    /// Notas adicionales sobre la planta (opcional).
    /// </summary>
    public string? Notas { get; set; }
    /// <summary>
    /// Estado actual de la planta en su ciclo de vida.
    /// </summary>
    public EstadoPlanta Estado { get; set; } = EstadoPlanta.Germinando;
    /// <summary>
    /// Número estimado de días hasta la cosecha desde la siembra.
    /// </summary>
    public int DiasHastaCosecha { get; set; } 
    
    /// <summary>
    /// Colección de riegos registrados para esta planta.
    /// </summary>
    public List<Riego> Riegos { get; set; } = new();
    /// <summary>
    /// Colección de tareas asociadas a esta planta.
    /// </summary>
    public List<Tarea> Tareas { get; set; } = new();
    /// <summary>
    /// Colección de cosechas registradas para esta planta.
    /// </summary>
    public List<Cosecha> Cosechas { get; set; } = new();
    
    /// <summary>
    /// Obtiene el número de días transcurridos desde la siembra.
    /// </summary>
    public int DiasDesdeSelección => (DateTime.Now - FechaSiembra).Days;
    /// <summary>
    /// Indica si la planta necesita riego. Una planta necesita riego si no tiene riegos registrados o si el último riego fue hace más de 2 días.
    /// </summary>
    public bool NecesitaRiego => Riegos.Count == 0 || Riegos.OrderByDescending(r => r.Fecha).FirstOrDefault()?.Fecha < DateTime.Now.AddDays(-2);
}

/// <summary>
/// Estados posibles en el ciclo de vida de una planta.
/// </summary>
public enum EstadoPlanta
{
    /// <summary>Planta en fase de germinación</summary>
    Germinando,
    /// <summary>Planta en fase de crecimiento vegetativo</summary>
    Creciendo,
    /// <summary>Planta en fase de floración</summary>
    Floreciendo,
    /// <summary>Planta produciendo frutos</summary>
    Fructificando,
    /// <summary>Planta lista para ser cosechada</summary>
    ListaParaCosechar,
    /// <summary>Planta ya cosechada</summary>
    Cosechada,
    /// <summary>Planta marchita o muerta</summary>
    Marchita
}