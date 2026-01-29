using PropertyChanged;
using System.Collections.ObjectModel;
using System.Windows.Input;
using HuertoApp.Models;
using HuertoApp.Data;
using HuertoApp.Services;
using Microsoft.EntityFrameworkCore;

namespace HuertoApp.ViewModels;

/// <summary>
/// ViewModel para la gestión de notas del huerto.
/// Maneja creación, visualización y eliminación de notas con categorías.
/// </summary>
[AddINotifyPropertyChangedInterface]
public class NotasViewModel
{
    private readonly HuertoContext _context;
    private readonly IValidationService _validationService;
    private readonly IImageService _imageService;

    public ObservableCollection<NotaHuerto> Notas { get; set; }
    
    public string TituloNuevaNota { get; set; }
    public string ContenidoNuevaNota { get; set; }
    public CategoriaNota CategoriaNuevaNota { get; set; } = CategoriaNota.General;
    public string FotoTemporal { get; set; }

    public ICommand AgregarNotaCommand { get; }
    public ICommand EliminarNotaCommand { get; }
    public ICommand TomarFotoCommand { get; }
    public ICommand SeleccionarGaleriaCommand { get; }
    public ICommand SeleccionarArchivoCommand { get; }

    public NotasViewModel(HuertoContext context, IValidationService validationService, IImageService imageService)
    {
        _context = context;
        _validationService = validationService;
        _imageService = imageService;
        
        AgregarNotaCommand = new Command(async () => await AgregarNota());
        EliminarNotaCommand = new Command<NotaHuerto>(async (n) => await EliminarNota(n));
        TomarFotoCommand = new Command(async () => await TomarFoto());
        SeleccionarGaleriaCommand = new Command(async () => await SeleccionarDeGaleria());
        SeleccionarArchivoCommand = new Command(async () => await SeleccionarArchivo());

        CargarNotas();
    }

    async void CargarNotas()
    {
        try
        {
            var notas = await _context.NotasHuerto
                .OrderByDescending(n => n.Fecha)
                .ToListAsync();
                
            Notas = new ObservableCollection<NotaHuerto>(notas);
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Error al cargar notas: {ex.Message}", "OK");
            Notas = new ObservableCollection<NotaHuerto>();
        }
    }

    async Task AgregarNota()
    {
        var validacionTitulo = _validationService.ValidateNotEmpty(TituloNuevaNota, "El título");
        if (!validacionTitulo.IsValid)
        {
            await Application.Current.MainPage.DisplayAlert("Validación", validacionTitulo.ErrorMessage, "OK");
            return;
        }

        var validacionContenido = _validationService.ValidateNotEmpty(ContenidoNuevaNota, "El contenido");
        if (!validacionContenido.IsValid)
        {
            await Application.Current.MainPage.DisplayAlert("Validación", validacionContenido.ErrorMessage, "OK");
            return;
        }

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
            var nuevaNota = new NotaHuerto
            {
                Titulo = TituloNuevaNota.Trim(),
                Contenido = ContenidoNuevaNota.Trim(),
                Categoria = CategoriaNuevaNota,
                Fecha = DateTime.Now,
                FotoPath = FotoTemporal
            };

            _context.NotasHuerto.Add(nuevaNota);
            await _context.SaveChangesAsync();

            Notas.Insert(0, nuevaNota);
            LimpiarFormulario();

            await Application.Current.MainPage.DisplayAlert("Éxito", $"¡Nota '{nuevaNota.Titulo}' guardada! ??", "OK");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo guardar la nota: {ex.Message}", "OK");
        }
    }

    async Task EliminarNota(NotaHuerto nota)
    {
        if (nota == null) return;

        var confirmar = await Application.Current.MainPage.DisplayAlert(
            "Confirmar eliminación", 
            $"¿Eliminar nota '{nota.Titulo}'?\n\nEsta acción no se puede deshacer.", 
            "Sí, eliminar", 
            "Cancelar");

        if (!confirmar) return;

        try
        {
            if (!string.IsNullOrEmpty(nota.FotoPath))
            {
                await _imageService.DeleteImageAsync(nota.FotoPath);
            }

            _context.NotasHuerto.Remove(nota);
            await _context.SaveChangesAsync();

            Notas.Remove(nota);
            
            await Application.Current.MainPage.DisplayAlert("Éxito", "Nota eliminada", "OK");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo eliminar la nota: {ex.Message}", "OK");
        }
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

    void LimpiarFormulario()
    {
        TituloNuevaNota = string.Empty;
        ContenidoNuevaNota = string.Empty;
        CategoriaNuevaNota = CategoriaNota.General;
        FotoTemporal = null;
    }
}
