using PropertyChanged;
using System.Collections.ObjectModel;
using System.Windows.Input;
using HuertoApp.Models;
using HuertoApp.Data;
using Microsoft.EntityFrameworkCore;

namespace HuertoApp.ViewModels;

[AddINotifyPropertyChangedInterface]
public class TareasViewModel
{
    private readonly HuertoContext _context;

    public ObservableCollection<Tarea> Tareas { get; set; }
    public ObservableCollection<Tarea> TareasPendientes { get; set; }
    public ObservableCollection<Tarea> TareasCompletadas { get; set; }
    public ObservableCollection<Planta> Plantas { get; set; }
    
    public string TituloNuevaTarea { get; set; }
    public string DescripcionNuevaTarea { get; set; }
    public DateTime? FechaVencimientoNueva { get; set; }
    public PrioridadTarea PrioridadNueva { get; set; } = PrioridadTarea.Media;
    public Planta PlantaSeleccionada { get; set; }
    
    public bool MostrarSoloPendientes { get; set; } = true;

    public ICommand AgregarTareaCommand { get; }
    public ICommand CompletarTareaCommand { get; }
    public ICommand EliminarTareaCommand { get; }
    public ICommand FiltrarTareasCommand { get; }

    public TareasViewModel(HuertoContext context)
    {
        _context = context;
        
        AgregarTareaCommand = new Command(async () => await AgregarTarea());
        CompletarTareaCommand = new Command<Tarea>(async (t) => await CompletarTarea(t));
        EliminarTareaCommand = new Command<Tarea>(async (t) => await EliminarTarea(t));
        FiltrarTareasCommand = new Command(() => CargarTareas());

        CargarDatos();
    }

    async void CargarDatos()
    {
        var plantas = await _context.Plantas.ToListAsync();
        Plantas = new ObservableCollection<Planta>(plantas);
        
        CargarTareas();
    }

    void CargarTareas()
    {
        var tareas = _context.Tareas
            .Include(t => t.Planta)
            .OrderBy(t => t.Completada)
            .ThenByDescending(t => t.Prioridad)
            .ThenBy(t => t.FechaVencimiento)
            .ToList();

        Tareas = new ObservableCollection<Tarea>(tareas);
        TareasPendientes = new ObservableCollection<Tarea>(tareas.Where(t => !t.Completada));
        TareasCompletadas = new ObservableCollection<Tarea>(tareas.Where(t => t.Completada));
    }

    async Task AgregarTarea()
    {
        if (string.IsNullOrWhiteSpace(TituloNuevaTarea))
        {
            await Application.Current.MainPage.DisplayAlert("Validación", "El título es obligatorio", "OK");
            return;
        }

        var nuevaTarea = new Tarea
        {
            Titulo = TituloNuevaTarea,
            Descripcion = DescripcionNuevaTarea,
            FechaCreacion = DateTime.Now,
            FechaVencimiento = FechaVencimientoNueva,
            Prioridad = PrioridadNueva,
            PlantaId = PlantaSeleccionada?.Id,
            Completada = false
        };

        _context.Tareas.Add(nuevaTarea);
        await _context.SaveChangesAsync();

        TareasPendientes.Insert(0, nuevaTarea);
        LimpiarFormulario();

        await Application.Current.MainPage.DisplayAlert("Éxito", "Tarea agregada", "OK");
    }

    async Task CompletarTarea(Tarea tarea)
    {
        if (tarea == null) return;

        tarea.Completada = !tarea.Completada;
        await _context.SaveChangesAsync();

        CargarTareas();
    }

    async Task EliminarTarea(Tarea tarea)
    {
        if (tarea == null) return;

        var confirmar = await Application.Current.MainPage.DisplayAlert(
            "Confirmar", 
            $"¿Eliminar tarea '{tarea.Titulo}'?", 
            "Sí", 
            "No");

        if (!confirmar) return;

        _context.Tareas.Remove(tarea);
        await _context.SaveChangesAsync();

        Tareas.Remove(tarea);
        if (tarea.Completada)
            TareasCompletadas.Remove(tarea);
        else
            TareasPendientes.Remove(tarea);
    }

    void LimpiarFormulario()
    {
        TituloNuevaTarea = string.Empty;
        DescripcionNuevaTarea = string.Empty;
        FechaVencimientoNueva = null;
        PrioridadNueva = PrioridadTarea.Media;
        PlantaSeleccionada = null;
    }
}
