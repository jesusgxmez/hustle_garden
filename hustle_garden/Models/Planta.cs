using System.ComponentModel.DataAnnotations;
using PropertyChanged;

namespace HuertoApp.Models;

[AddINotifyPropertyChangedInterface]
public class Planta
{
    [Key]
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Variedad { get; set; }
    public DateTime FechaSiembra { get; set; }
    public string FotoPath { get; set; }
}