using PropertyChanged;
using System.Collections.ObjectModel;
using System.Windows.Input;
using HuertoApp.Models;
using HuertoApp.Data;
using HuertoApp.Services;
using Microsoft.EntityFrameworkCore;

namespace HuertoApp.ViewModels;

[AddINotifyPropertyChangedInterface]
public class HuertoViewModel
{
    private readonly HuertoContext _context;
    private readonly IValidationService _validationService;
    private readonly IImageService _imageService;

    public ObservableCollection<Planta> Plantas { get; set; }
    public ObservableCollection<Planta> PlantasQueSiembraRiego { get; set; }
    
    public string NombreNuevaPlanta { get; set; }
    public string VariedadNuevaPlanta { get; set; }
    public string UbicacionNuevaPlanta { get; set; }
    public int DiasHastaCosecha { get; set; }
    public string FotoTemporal { get; set; }
    
    public Planta PlantaSeleccionada { get; set; }
    
    public string FiltroEstado { get; set; } = "Todas";

    public ICommand BorrarPlantaCommand { get; }
    public ICommand TomarFotoCommand { get; }
    public ICommand GuardarPlantaCommand { get; }
    public ICommand EditarPlantaCommand { get; }
    public ICommand CambiarEstadoPlantaCommand { get; }
    public ICommand FiltrarPorEstadoCommand { get; }
    public ICommand VerDetallePlantaCommand { get; }
    public ICommand SeleccionarGaleriaCommand { get; }
    public ICommand SeleccionarArchivoCommand { get; }
    public ICommand EliminarFotoCommand { get; }
    public ICommand MostrarPopupAgregarPlantaCommand { get; }

    public HuertoViewModel(HuertoContext context, IValidationService validationService, IImageService imageService)
    {
        _context = context;
        _validationService = validationService;
        _imageService = imageService;
        
        TomarFotoCommand = new Command(async () => await TomarFoto());
        SeleccionarGaleriaCommand = new Command(async () => await SeleccionarDeGaleria());
        SeleccionarArchivoCommand = new Command(async () => await SeleccionarArchivo());
        EliminarFotoCommand = new Command(EliminarFoto);
        GuardarPlantaCommand = new Command(async () => await GuardarPlanta());
        BorrarPlantaCommand = new Command<Planta>(async (p) => await BorrarPlanta(p));
        EditarPlantaCommand = new Command<Planta>(async (p) => await EditarPlanta(p));
        CambiarEstadoPlantaCommand = new Command<Planta>(async (p) => await CambiarEstadoPlanta(p));
        FiltrarPorEstadoCommand = new Command<string>((estado) => FiltrarPlantas(estado));
        VerDetallePlantaCommand = new Command<Planta>(async (p) => await VerDetallePlanta(p));
        MostrarPopupAgregarPlantaCommand = new Command(async () => await MostrarModalAgregarPlanta());

        CargarPlantas();
    }

    async Task TomarFoto()
    {
        var resultado = await _imageService.CapturePhotoAsync();
        
        if (resultado.IsSuccess)
        {
            FotoTemporal = resultado.ImagePath;
        }
        else if (!resultado.IsCancelled)
        {
            await Application.Current.MainPage.DisplayAlert("Error", resultado.ErrorMessage, "OK");
        }
    }
    
    async Task SeleccionarDeGaleria()
    {
        var resultado = await _imageService.PickPhotoAsync();
        
        if (resultado.IsSuccess)
        {
            FotoTemporal = resultado.ImagePath;
        }
        else if (!resultado.IsCancelled)
        {
            await Application.Current.MainPage.DisplayAlert("Error", resultado.ErrorMessage, "OK");
        }
    }

    async Task SeleccionarArchivo()
    {
        var resultado = await _imageService.PickPhotoFromFileSystemAsync();
        
        if (resultado.IsSuccess)
        {
            FotoTemporal = resultado.ImagePath;
        }
        else if (!resultado.IsCancelled)
        {
            await Application.Current.MainPage.DisplayAlert("Error", resultado.ErrorMessage, "OK");
        }
    }

    void EliminarFoto()
    {
        if (!string.IsNullOrEmpty(FotoTemporal))
        {
            _ = _imageService.DeleteImageAsync(FotoTemporal);
            FotoTemporal = null;
        }
    }

    async Task MostrarModalAgregarPlanta()
    {
        var modalPage = new hustle_garden.Views.AgregarPlantaPage(this);
        await Application.Current.MainPage.Navigation.PushModalAsync(modalPage);
    }

    public async Task<bool> GuardarPlantaDesdeModal()
    {
        return await GuardarPlanta();
    }

    async Task<bool> GuardarPlanta()
    {
        var nombreValidacion = _validationService.ValidatePlantName(NombreNuevaPlanta);
        if (!nombreValidacion.IsValid)
        {
            await Application.Current.MainPage.DisplayAlert("Validación", nombreValidacion.ErrorMessage, "OK");
            return false;
        }

        var diasValidacion = ValidationService.ValidateDaysToHarvest(DiasHastaCosecha);
        if (!diasValidacion.IsValid)
        {
            await Application.Current.MainPage.DisplayAlert("Validación", diasValidacion.ErrorMessage, "OK");
            return false;
        }

        if (!string.IsNullOrEmpty(FotoTemporal))
        {
            var imagenValidacion = _validationService.ValidateImagePath(FotoTemporal);
            if (!imagenValidacion.IsValid)
            {
                await Application.Current.MainPage.DisplayAlert("Validación", imagenValidacion.ErrorMessage, "OK");
                return false;
            }
        }

        try
        {
            var nueva = new Planta
            {
                Nombre = NombreNuevaPlanta.Trim(),
                Variedad = string.IsNullOrWhiteSpace(VariedadNuevaPlanta) ? null : VariedadNuevaPlanta.Trim(),
                Ubicacion = string.IsNullOrWhiteSpace(UbicacionNuevaPlanta) ? null : UbicacionNuevaPlanta.Trim(),
                FotoPath = FotoTemporal,
                FechaSiembra = DateTime.Now,
                DiasHastaCosecha = DiasHastaCosecha,
                FechaCosechaEstimada = DiasHastaCosecha > 0 ? DateTime.Now.AddDays(DiasHastaCosecha) : null,
                Estado = EstadoPlanta.Germinando
            };

            _context.Plantas.Add(nueva);
            await _context.SaveChangesAsync();

            Plantas.Add(nueva);

            LimpiarFormulario();
            
            await Application.Current.MainPage.DisplayAlert("Éxito", $"¡{nueva.Nombre} ha sido añadida exitosamente! 🌱", "OK");
            
            return true;
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo guardar la planta: {ex.Message}", "OK");
            return false;
        }
    }

    async Task EditarPlanta(Planta planta)
    {
        if (planta == null) return;
        
        await Shell.Current.GoToAsync($"DetallePlantaPage?plantaId={planta.Id}");
    }

    async void CargarPlantas()
    {
        try
        {
            var datos = await _context.Plantas
                .Include(p => p.Riegos)
                .Include(p => p.Tareas)
                .Include(p => p.Cosechas)
                .ToListAsync();
                
            Plantas = new ObservableCollection<Planta>(datos);
            PlantasQueSiembraRiego = new ObservableCollection<Planta>(datos.Where(p => p.NecesitaRiego));
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Error al cargar plantas: {ex.Message}", "OK");
            Plantas = new ObservableCollection<Planta>();
            PlantasQueSiembraRiego = new ObservableCollection<Planta>();
        }
    }

    async Task BorrarPlanta(Planta planta)
    {
        if (planta == null) return;

        var confirmar = await Application.Current.MainPage.DisplayAlert(
            "Confirmar eliminación", 
            $"¿Estás seguro de que deseas eliminar '{planta.Nombre}'?\n\nEsta acción no se puede deshacer.", 
            "Sí, eliminar", 
            "Cancelar");
            
        if (!confirmar) return;

        try
        {
            if (!string.IsNullOrEmpty(planta.FotoPath))
            {
                await _imageService.DeleteImageAsync(planta.FotoPath);
            }

            _context.Plantas.Remove(planta);
            await _context.SaveChangesAsync();

            Plantas.Remove(planta);
            
            await Application.Current.MainPage.DisplayAlert("Éxito", $"'{planta.Nombre}' ha sido eliminada", "OK");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo eliminar la planta: {ex.Message}", "OK");
        }
    }
    
    async Task CambiarEstadoPlanta(Planta planta)
    {
        if (planta == null) return;
        
        var estados = Enum.GetNames(typeof(EstadoPlanta));
        var resultado = await Application.Current.MainPage.DisplayActionSheet(
            $"Cambiar estado de '{planta.Nombre}'", 
            "Cancelar", 
            null, 
            estados);
            
        if (resultado != "Cancelar" && Enum.TryParse<EstadoPlanta>(resultado, out var nuevoEstado))
        {
            try
            {
                planta.Estado = nuevoEstado;
                await _context.SaveChangesAsync();
                
                await Application.Current.MainPage.DisplayAlert("Éxito", $"Estado actualizado a '{nuevoEstado}'", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo actualizar el estado: {ex.Message}", "OK");
            }
        }
    }
    
    void FiltrarPlantas(string estado)
    {
        if (string.IsNullOrWhiteSpace(estado))
        {
            estado = "Todas";
        }

        FiltroEstado = estado;
        
        try
        {
            if (estado == "Todas")
            {
                CargarPlantas();
            }
            else if (Enum.TryParse<EstadoPlanta>(estado, out var estadoEnum))
            {
                var plantasFiltradas = _context.Plantas
                    .Where(p => p.Estado == estadoEnum)
                    .ToList();
                Plantas = new ObservableCollection<Planta>(plantasFiltradas);
            }
        }
        catch (Exception ex)
        {
            Application.Current.MainPage.DisplayAlert("Error", $"Error al filtrar plantas: {ex.Message}", "OK");
        }
    }
    
    async Task VerDetallePlanta(Planta planta)
    {
        if (planta == null) return;
        
        await Shell.Current.GoToAsync($"DetallePlantaPage?plantaId={planta.Id}");
    }
    
    void LimpiarFormulario()
    {
        NombreNuevaPlanta = string.Empty;
        VariedadNuevaPlanta = string.Empty;
        UbicacionNuevaPlanta = string.Empty;
        DiasHastaCosecha = 0;
        FotoTemporal = null;
    }
}