using System.ComponentModel.DataAnnotations;
using PropertyChanged;

namespace HuertoApp.Models;

[AddINotifyPropertyChangedInterface]
public class NotaHuerto
{
    [Key]
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public CategoriaNota Categoria { get; set; } = CategoriaNota.General;
    public string? FotoPath { get; set; }
}

public enum CategoriaNota
{
    General,
    Clima,
    Plagas,
    Fertilizacion,
    Observacion,
    Recordatorio
}
