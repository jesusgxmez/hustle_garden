using System.ComponentModel.DataAnnotations;
using PropertyChanged;

namespace HuertoApp.Models;

[AddINotifyPropertyChangedInterface]
public class Planta
{
    [Key]
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Variedad { get; set; }
    public DateTime FechaSiembra { get; set; }
    public DateTime? FechaCosechaEstimada { get; set; }
    public string? FotoPath { get; set; }
    public string? Ubicacion { get; set; } 
    public string? Notas { get; set; }
    public EstadoPlanta Estado { get; set; } = EstadoPlanta.Germinando;
    public int DiasHastaCosecha { get; set; } 
    
    public List<Riego> Riegos { get; set; } = new();
    public List<Tarea> Tareas { get; set; } = new();
    public List<Cosecha> Cosechas { get; set; } = new();
    
    public int DiasDesdeSelección => (DateTime.Now - FechaSiembra).Days;
    public bool NecesitaRiego => Riegos.Count == 0 || Riegos.OrderByDescending(r => r.Fecha).FirstOrDefault()?.Fecha < DateTime.Now.AddDays(-2);
}

public enum EstadoPlanta
{
    Germinando,
    Creciendo,
    Floreciendo,
    Fructificando,
    ListaParaCosechar,
    Cosechada,
    Marchita
}