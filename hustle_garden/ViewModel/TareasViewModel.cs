using PropertyChanged;
using System.Collections.ObjectModel;
using System.Windows.Input;
using HuertoApp.Models;
using HuertoApp.Data;
using HuertoApp.Services;
using Microsoft.EntityFrameworkCore;

namespace HuertoApp.ViewModels;

[AddINotifyPropertyChangedInterface]
public class TareasViewModel
{
    private readonly HuertoContext _context;
    private readonly IValidationService _validationService;

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

    public TareasViewModel(HuertoContext context, IValidationService validationService)
    {
        _context = context;
        _validationService = validationService;
        
        AgregarTareaCommand = new Command(async () => await AgregarTarea());
        CompletarTareaCommand = new Command<Tarea>(async (t) => await CompletarTarea(t));
        EliminarTareaCommand = new Command<Tarea>(async (t) => await EliminarTarea(t));
        FiltrarTareasCommand = new Command(() => CargarTareas());

        CargarDatos();
    }

    async void CargarDatos()
    {
        try
        {
            var plantas = await _context.Plantas.ToListAsync();
            Plantas = new ObservableCollection<Planta>(plantas);
            
            CargarTareas();
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Error al cargar datos: {ex.Message}", "OK");
            Plantas = new ObservableCollection<Planta>();
            Tareas = new ObservableCollection<Tarea>();
            TareasPendientes = new ObservableCollection<Tarea>();
            TareasCompletadas = new ObservableCollection<Tarea>();
        }
    }

    void CargarTareas()
    {
        try
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
        catch (Exception ex)
        {
            Application.Current.MainPage.DisplayAlert("Error", $"Error al cargar tareas: {ex.Message}", "OK");
            Tareas = new ObservableCollection<Tarea>();
            TareasPendientes = new ObservableCollection<Tarea>();
            TareasCompletadas = new ObservableCollection<Tarea>();
        }
    }

    async Task AgregarTarea()
    {
        var validacion = _validationService.ValidateNotEmpty(TituloNuevaTarea, "El título");
        if (!validacion.IsValid)
        {
            await Application.Current.MainPage.DisplayAlert("Validación", validacion.ErrorMessage, "OK");
            return;
        }

        if (FechaVencimientoNueva.HasValue)
        {
            var fechaValidacion = _validationService.ValidateDateRange(
                FechaVencimientoNueva.Value, 
                DateTime.Now.Date, 
                DateTime.Now.AddYears(5), 
                "La fecha de vencimiento");
            
            if (!fechaValidacion.IsValid)
            {
                await Application.Current.MainPage.DisplayAlert("Validación", fechaValidacion.ErrorMessage, "OK");
                return;
            }
        }

        try
        {
            var nuevaTarea = new Tarea
            {
                Titulo = TituloNuevaTarea.Trim(),
                Descripcion = string.IsNullOrWhiteSpace(DescripcionNuevaTarea) ? null : DescripcionNuevaTarea.Trim(),
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

            await Application.Current.MainPage.DisplayAlert("Éxito", $"¡Tarea '{nuevaTarea.Titulo}' agregada! ?", "OK");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo agregar la tarea: {ex.Message}", "OK");
        }
    }

    async Task CompletarTarea(Tarea tarea)
    {
        if (tarea == null) return;

        try
        {
            tarea.Completada = !tarea.Completada;
            await _context.SaveChangesAsync();

            CargarTareas();
            
            var mensaje = tarea.Completada ? "Tarea completada ?" : "Tarea marcada como pendiente";
            await Application.Current.MainPage.DisplayAlert("Éxito", mensaje, "OK");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo actualizar la tarea: {ex.Message}", "OK");
            tarea.Completada = !tarea.Completada;
        }
    }

    async Task EliminarTarea(Tarea tarea)
    {
        if (tarea == null) return;

        var confirmar = await Application.Current.MainPage.DisplayAlert(
            "Confirmar eliminación", 
            $"¿Eliminar tarea '{tarea.Titulo}'?\n\nEsta acción no se puede deshacer.", 
            "Sí, eliminar", 
            "Cancelar");

        if (!confirmar) return;

        try
        {
            _context.Tareas.Remove(tarea);
            await _context.SaveChangesAsync();

            Tareas.Remove(tarea);
            if (tarea.Completada)
                TareasCompletadas.Remove(tarea);
            else
                TareasPendientes.Remove(tarea);
                
            await Application.Current.MainPage.DisplayAlert("Éxito", "Tarea eliminada", "OK");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo eliminar la tarea: {ex.Message}", "OK");
        }
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
