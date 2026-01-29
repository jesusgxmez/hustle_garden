using PropertyChanged;
using System.Collections.ObjectModel;
using System.Windows.Input;
using HuertoApp.Models;
using HuertoApp.Data;
using Microsoft.EntityFrameworkCore;

namespace HuertoApp.ViewModels;

/// <summary>
/// ViewModel para la visualización de estadísticas del huerto.
/// Muestra métricas agregadas, cosechas por mes y plantas más productivas.
/// </summary>
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
        CosechasPorMes = new ObservableCollection<EstadisticaMensual>();
        PlantasMasProductivas = new ObservableCollection<PlantaMasProductiva>();
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
            .Take(6)
            .ToList();

        CosechasPorMes.Clear();
        foreach (var item in cosechasPorMes)
        {
            CosechasPorMes.Add(item);
        }

        var plantasProductivas = plantas
            .Where(p => p.Cosechas.Any())
            .Select(p => new PlantaMasProductiva
            {
                NombrePlanta = p.Nombre,
                TotalKg = p.Cosechas.Sum(c => c.CantidadKg),
                NumeroCosechas = p.Cosechas.Count
            })
            .OrderByDescending(p => p.TotalKg)
            .Take(5)
            .ToList();

        PlantasMasProductivas.Clear();
        foreach (var item in plantasProductivas)
        {
            PlantasMasProductivas.Add(item);
        }
    }
}

/// <summary>
/// Representa datos estadísticos mensuales de cosechas.
/// </summary>
[AddINotifyPropertyChangedInterface]
public class EstadisticaMensual
{
    /// <summary>
    /// Mes en formato "MM/YYYY".
    /// </summary>
    public string Mes { get; set; }
    /// <summary>
    /// Cantidad total cosechada en el mes (kg).
    /// </summary>
    public double Cantidad { get; set; }
}

/// <summary>
/// Representa datos de productividad de una planta.
/// </summary>
[AddINotifyPropertyChangedInterface]
public class PlantaMasProductiva
{
    /// <summary>
    /// Nombre de la planta.
    /// </summary>
    public string NombrePlanta { get; set; }
    /// <summary>
    /// Total de kilogramos cosechados de esta planta.
    /// </summary>
    public double TotalKg { get; set; }
    /// <summary>
    /// Número de cosechas realizadas.
    /// </summary>
    public int NumeroCosechas { get; set; }
}
