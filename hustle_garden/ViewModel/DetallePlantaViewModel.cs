using PropertyChanged;
using System.Collections.ObjectModel;
using System.Windows.Input;
using HuertoApp.Models;
using HuertoApp.Data;
using HuertoApp.Services;
using Microsoft.EntityFrameworkCore;

namespace HuertoApp.ViewModels;

[AddINotifyPropertyChangedInterface]
[QueryProperty(nameof(PlantaId), "plantaId")]
public class DetallePlantaViewModel
{
    private readonly HuertoContext _context;
    private readonly IValidationService _validationService;
    private readonly IImageService _imageService;

    public int PlantaId { get; set; }
    public Planta Planta { get; set; }
    public ObservableCollection<Riego> Riegos { get; set; }
    public ObservableCollection<Cosecha> Cosechas { get; set; }
    
    public double CantidadRiegoNuevo { get; set; }
    public double CantidadCosechaNueva { get; set; }
    public string CalidadCosechaNueva { get; set; }
    public int SelectedIndexCalidad { get; set; } = -1;
    public string NotasCosechaNueva { get; set; }
    public string FotoTemporal { get; set; }
    
    public ICommand RegistrarRiegoCommand { get; }
    public ICommand RegistrarCosechaCommand { get; }
    public ICommand CambiarEstadoCommand { get; }
    public ICommand EditarPlantaCommand { get; }
    public ICommand TomarFotoCosechaCommand { get; }
    public ICommand SeleccionarFotoCosechaGaleriaCommand { get; }
    public ICommand SeleccionarFotoCosechaArchivoCommand { get; }
    public ICommand VerHistorialRiegosCommand { get; }

    public DetallePlantaViewModel(HuertoContext context, IValidationService validationService, IImageService imageService)
    {
        _context = context;
        _validationService = validationService;
        _imageService = imageService;
        
        RegistrarRiegoCommand = new Command(async () => await RegistrarRiego());
        RegistrarCosechaCommand = new Command(async () => await RegistrarCosecha());
        CambiarEstadoCommand = new Command(async () => await CambiarEstado());
        EditarPlantaCommand = new Command(async () => await EditarPlanta());
        TomarFotoCosechaCommand = new Command(async () => await TomarFoto());
        SeleccionarFotoCosechaGaleriaCommand = new Command(async () => await SeleccionarFotoGaleria());
        SeleccionarFotoCosechaArchivoCommand = new Command(async () => await SeleccionarFotoArchivo());
        VerHistorialRiegosCommand = new Command(async () => await VerHistorialRiegos());
    }

    public async Task CargarDatos()
    {
        try
        {
            Planta = await _context.Plantas
                .Include(p => p.Riegos)
                .Include(p => p.Cosechas)
                .Include(p => p.Tareas)
                .FirstOrDefaultAsync(p => p.Id == PlantaId);

            if (Planta != null)
            {
                Riegos = new ObservableCollection<Riego>(Planta.Riegos.OrderByDescending(r => r.Fecha).Take(5));
                Cosechas = new ObservableCollection<Cosecha>(Planta.Cosechas.OrderByDescending(c => c.Fecha));
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se encontró la planta", "OK");
                await Shell.Current.GoToAsync("..");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Error al cargar datos: {ex.Message}", "OK");
        }
    }

    async Task RegistrarRiego()
    {
        var validacion = ValidationService.ValidateWaterAmount(CantidadRiegoNuevo);
        if (!validacion.IsValid)
        {
            await Application.Current.MainPage.DisplayAlert("Validación", validacion.ErrorMessage, "OK");
            return;
        }

        try
        {
            var nuevoRiego = new Riego
            {
                PlantaId = PlantaId,
                Fecha = DateTime.Now,
                CantidadLitros = Math.Round(CantidadRiegoNuevo, 2)
            };

            _context.Riegos.Add(nuevoRiego);
            await _context.SaveChangesAsync();

            Riegos.Insert(0, nuevoRiego);
            if (Riegos.Count > 5)
            {
                Riegos.RemoveAt(Riegos.Count - 1);
            }
            
            CantidadRiegoNuevo = 0;

            await Application.Current.MainPage.DisplayAlert("Éxito", $"Riego de {nuevoRiego.CantidadLitros}L registrado ??", "OK");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo registrar el riego: {ex.Message}", "OK");
        }
    }

    async Task RegistrarCosecha()
    {
        var validacion = ValidationService.ValidateHarvestAmount(CantidadCosechaNueva);
        if (!validacion.IsValid)
        {
            await Application.Current.MainPage.DisplayAlert("Validación", validacion.ErrorMessage, "OK");
            return;
        }

        if (SelectedIndexCalidad < 0)
        {
            await Application.Current.MainPage.DisplayAlert("Validación", "Por favor selecciona la calidad de la cosecha", "OK");
            return;
        }

        var calidades = new[] { CalidadCosecha.Excelente, CalidadCosecha.Buena, CalidadCosecha.Regular, CalidadCosecha.Pobre };
        var calidadEnum = calidades[SelectedIndexCalidad];

        if (!string.IsNullOrEmpty(FotoTemporal))
        {
            var imagenValidacion = _validationService.ValidateImagePath(FotoTemporal);
            if (!imagenValidacion.IsValid)
            {
                await Application.Current.MainPage.DisplayAlert("Validación", imagenValidacion.ErrorMessage, "OK");
                return;
            }
        }

        try
        {
            var nuevaCosecha = new Cosecha
            {
                PlantaId = PlantaId,
                Fecha = DateTime.Now,
                CantidadKg = Math.Round(CantidadCosechaNueva, 2),
                Calidad = calidadEnum,
                Notas = string.IsNullOrWhiteSpace(NotasCosechaNueva) ? null : NotasCosechaNueva.Trim(),
                FotoPath = FotoTemporal
            };

            _context.Cosechas.Add(nuevaCosecha);
            
            Planta.Estado = EstadoPlanta.Cosechada;
            
            await _context.SaveChangesAsync();

            Cosechas.Insert(0, nuevaCosecha);
            LimpiarFormularioCosecha();

            await Application.Current.MainPage.DisplayAlert("Éxito", $"¡Cosecha registrada! ??\n{nuevaCosecha.CantidadKg} kg de calidad {nuevaCosecha.Calidad}", "OK");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo registrar la cosecha: {ex.Message}", "OK");
        }
    }

    async Task CambiarEstado()
    {
        var estados = Enum.GetNames(typeof(EstadoPlanta));
        var resultado = await Application.Current.MainPage.DisplayActionSheet(
            $"Cambiar estado de '{Planta?.Nombre}'", 
            "Cancelar", 
            null, 
            estados);

        if (resultado != "Cancelar" && Enum.TryParse<EstadoPlanta>(resultado, out var nuevoEstado))
        {
            try
            {
                Planta.Estado = nuevoEstado;
                await _context.SaveChangesAsync();
                
                await Application.Current.MainPage.DisplayAlert("Éxito", $"Estado actualizado a '{nuevoEstado}'", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo cambiar el estado: {ex.Message}", "OK");
            }
        }
    }

    async Task EditarPlanta()
    {
        await Application.Current.MainPage.DisplayAlert("Info", "Función de edición en desarrollo", "OK");
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
    
    async Task SeleccionarFotoGaleria()
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
    
    async Task SeleccionarFotoArchivo()
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
    
    async Task VerHistorialRiegos()
    {
        await Shell.Current.GoToAsync($"HistorialRiegosPage?plantaId={PlantaId}");
    }
    
    void LimpiarFormularioCosecha()
    {
        CantidadCosechaNueva = 0;
        CalidadCosechaNueva = null;
        SelectedIndexCalidad = -1;
        NotasCosechaNueva = string.Empty;
        FotoTemporal = null;
    }
}
