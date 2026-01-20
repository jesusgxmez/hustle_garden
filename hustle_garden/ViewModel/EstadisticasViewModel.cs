using PropertyChanged;
using System.Collections.ObjectModel;
using System.Windows.Input;
using HuertoApp.Models;
using HuertoApp.Data;
using Microsoft.EntityFrameworkCore;

namespace HuertoApp.ViewModels;

[AddINotifyPropertyChangedInterface]
public class EstadisticasViewModel
{
    private readonly HuertoContext _context;

    public int TotalPlantas { get; set; }
    public int PlantasActivas { get; set; }
    public int TotalCosechas { get; set; }
    public double TotalKgCosechados { get; set; }
    public int TareasPendientes { get; set; }
    public int PlantasQueNecesitanRiego { get; set; }
    
    public ObservableCollection<EstadisticaMensual> CosechasPorMes { get; set; }
    public ObservableCollection<PlantaMasProductiva> PlantasMasProductivas { get; set; }

    public ICommand RefrescarCommand { get; }

    public EstadisticasViewModel(HuertoContext context)
    {
        _context = context;
        RefrescarCommand = new Command(async () => await CargarEstadisticas());
        
        _ = CargarEstadisticas();
    }

    async Task CargarEstadisticas()
    {
        var plantas = await _context.Plantas
            .Include(p => p.Riegos)
            .Include(p => p.Cosechas)
            .ToListAsync();
            
        var tareas = await _context.Tareas.ToListAsync();
        var cosechas = await _context.Cosechas.ToListAsync();

        TotalPlantas = plantas.Count;
        PlantasActivas = plantas.Count(p => p.Estado != EstadoPlanta.Cosechada && p.Estado != EstadoPlanta.Marchita);
        TotalCosechas = cosechas.Count;
        TotalKgCosechados = cosechas.Sum(c => c.CantidadKg);
        TareasPendientes = tareas.Count(t => !t.Completada);
        PlantasQueNecesitanRiego = plantas.Count(p => p.NecesitaRiego);

        var cosechasPorMes = cosechas
            .GroupBy(c => new { c.Fecha.Year, c.Fecha.Month })
            .Select(g => new EstadisticaMensual
            {
                Mes = $"{g.Key.Month}/{g.Key.Year}",
                Cantidad = g.Sum(c => c.CantidadKg)
            })
            .OrderByDescending(e => e.Mes)
            .Take(6);

        CosechasPorMes = new ObservableCollection<EstadisticaMensual>(cosechasPorMes);

        var plantasProductivas = plantas
            .Where(p => p.Cosechas.Any())
            .Select(p => new PlantaMasProductiva
            {
                NombrePlanta = p.Nombre,
                TotalKg = p.Cosechas.Sum(c => c.CantidadKg),
                NumeroCosechas = p.Cosechas.Count
            })
            .OrderByDescending(p => p.TotalKg)
            .Take(5);

        PlantasMasProductivas = new ObservableCollection<PlantaMasProductiva>(plantasProductivas);
    }
}

[AddINotifyPropertyChangedInterface]
public class EstadisticaMensual
{
    public string Mes { get; set; }
    public double Cantidad { get; set; }
}

[AddINotifyPropertyChangedInterface]
public class PlantaMasProductiva
{
    public string NombrePlanta { get; set; }
    public double TotalKg { get; set; }
    public int NumeroCosechas { get; set; }
}
