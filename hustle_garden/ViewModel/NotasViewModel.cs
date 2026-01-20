using PropertyChanged;
using System.Collections.ObjectModel;
using System.Windows.Input;
using HuertoApp.Models;
using HuertoApp.Data;
using Microsoft.EntityFrameworkCore;

namespace HuertoApp.ViewModels;

[AddINotifyPropertyChangedInterface]
public class NotasViewModel
{
    private readonly HuertoContext _context;

    public ObservableCollection<NotaHuerto> Notas { get; set; }
    
    public string TituloNuevaNota { get; set; }
    public string ContenidoNuevaNota { get; set; }
    public CategoriaNota CategoriaNuevaNota { get; set; } = CategoriaNota.General;
    public string FotoTemporal { get; set; }

    public ICommand AgregarNotaCommand { get; }
    public ICommand EliminarNotaCommand { get; }
    public ICommand TomarFotoCommand { get; }
    public ICommand SeleccionarGaleriaCommand { get; }

    public NotasViewModel(HuertoContext context)
    {
        _context = context;
        
        AgregarNotaCommand = new Command(async () => await AgregarNota());
        EliminarNotaCommand = new Command<NotaHuerto>(async (n) => await EliminarNota(n));
        TomarFotoCommand = new Command(async () => await TomarFoto());
        SeleccionarGaleriaCommand = new Command(async () => await SeleccionarDeGaleria());

        CargarNotas();
    }

    async void CargarNotas()
    {
        var notas = await _context.NotasHuerto
            .OrderByDescending(n => n.Fecha)
            .ToListAsync();
            
        Notas = new ObservableCollection<NotaHuerto>(notas);
    }

    async Task AgregarNota()
    {
        if (string.IsNullOrWhiteSpace(TituloNuevaNota))
        {
            await Application.Current.MainPage.DisplayAlert("Validación", "El título es obligatorio", "OK");
            return;
        }

        var nuevaNota = new NotaHuerto
        {
            Titulo = TituloNuevaNota,
            Contenido = ContenidoNuevaNota,
            Categoria = CategoriaNuevaNota,
            Fecha = DateTime.Now,
            FotoPath = FotoTemporal
        };

        _context.NotasHuerto.Add(nuevaNota);
        await _context.SaveChangesAsync();

        Notas.Insert(0, nuevaNota);
        LimpiarFormulario();

        await Application.Current.MainPage.DisplayAlert("Éxito", "Nota guardada", "OK");
    }

    async Task EliminarNota(NotaHuerto nota)
    {
        if (nota == null) return;

        var confirmar = await Application.Current.MainPage.DisplayAlert(
            "Confirmar", 
            $"¿Eliminar nota '{nota.Titulo}'?", 
            "Sí", 
            "No");

        if (!confirmar) return;

        _context.NotasHuerto.Remove(nota);
        await _context.SaveChangesAsync();

        Notas.Remove(nota);
    }

    async Task TomarFoto()
    {
        try
        {
            var foto = await MediaPicker.Default.CapturePhotoAsync();
            if (foto != null)
            {
                var rutaLocal = Path.Combine(FileSystem.AppDataDirectory, foto.FileName);
                using var stream = await foto.OpenReadAsync();
                using var newStream = File.OpenWrite(rutaLocal);
                await stream.CopyToAsync(newStream);

                FotoTemporal = rutaLocal;
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo tomar la foto: {ex.Message}", "OK");
        }
    }

    async Task SeleccionarDeGaleria()
    {
        try
        {
            var foto = await MediaPicker.Default.PickPhotoAsync();
            if (foto != null)
            {
                var rutaLocal = Path.Combine(FileSystem.AppDataDirectory, $"{Guid.NewGuid()}{Path.GetExtension(foto.FileName)}");
                using var stream = await foto.OpenReadAsync();
                using var newStream = File.OpenWrite(rutaLocal);
                await stream.CopyToAsync(newStream);

                FotoTemporal = rutaLocal;
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo seleccionar la foto: {ex.Message}", "OK");
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
