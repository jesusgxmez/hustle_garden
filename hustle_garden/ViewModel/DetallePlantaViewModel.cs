using PropertyChanged;
using System.Collections.ObjectModel;
using System.Windows.Input;
using HuertoApp.Models;
using HuertoApp.Data;
using Microsoft.EntityFrameworkCore;

namespace HuertoApp.ViewModels;

[AddINotifyPropertyChangedInterface]
[QueryProperty(nameof(PlantaId), "plantaId")]
public class DetallePlantaViewModel
{
    private readonly HuertoContext _context;

    public int PlantaId { get; set; }
    public Planta Planta { get; set; }
    public ObservableCollection<Riego> Riegos { get; set; }
    public ObservableCollection<Cosecha> Cosechas { get; set; }
    
    public double CantidadRiegoNuevo { get; set; }
    public double CantidadCosechaNueva { get; set; }
    public CalidadCosecha CalidadCosechaNueva { get; set; } = CalidadCosecha.Buena;
    public string NotasCosechaNueva { get; set; }
    public string FotoTemporal { get; set; }
    
    public ICommand RegistrarRiegoCommand { get; }
    public ICommand RegistrarCosechaCommand { get; }
    public ICommand CambiarEstadoCommand { get; }
    public ICommand EditarPlantaCommand { get; }
    public ICommand TomarFotoCosechaCommand { get; }
    public ICommand VerHistorialRiegosCommand { get; }

    public DetallePlantaViewModel(HuertoContext context)
    {
        _context = context;
        
        RegistrarRiegoCommand = new Command(async () => await RegistrarRiego());
        RegistrarCosechaCommand = new Command(async () => await RegistrarCosecha());
        CambiarEstadoCommand = new Command(async () => await CambiarEstado());
        EditarPlantaCommand = new Command(async () => await EditarPlanta());
        TomarFotoCosechaCommand = new Command(async () => await TomarFoto());
        VerHistorialRiegosCommand = new Command(async () => await VerHistorialRiegos());
    }

    public async Task CargarDatos()
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
    }

    async Task RegistrarRiego()
    {
        if (CantidadRiegoNuevo <= 0)
        {
            await Application.Current.MainPage.DisplayAlert("Validación", "Ingresa la cantidad de agua en litros", "OK");
            return;
        }

        var nuevoRiego = new Riego
        {
            PlantaId = PlantaId,
            Fecha = DateTime.Now,
            CantidadLitros = CantidadRiegoNuevo
        };

        _context.Riegos.Add(nuevoRiego);
        await _context.SaveChangesAsync();

        Riegos.Insert(0, nuevoRiego);
        CantidadRiegoNuevo = 0;

        await Application.Current.MainPage.DisplayAlert("Éxito", "Riego registrado", "OK");
    }

    async Task RegistrarCosecha()
    {
        if (CantidadCosechaNueva <= 0)
        {
            await Application.Current.MainPage.DisplayAlert("Validación", "Ingresa la cantidad cosechada en kg", "OK");
            return;
        }

        var nuevaCosecha = new Cosecha
        {
            PlantaId = PlantaId,
            Fecha = DateTime.Now,
            CantidadKg = CantidadCosechaNueva,
            Calidad = CalidadCosechaNueva,
            Notas = NotasCosechaNueva,
            FotoPath = FotoTemporal
        };

        _context.Cosechas.Add(nuevaCosecha);
        
        Planta.Estado = EstadoPlanta.Cosechada;
        
        await _context.SaveChangesAsync();

        Cosechas.Insert(0, nuevaCosecha);
        LimpiarFormularioCosecha();

        await Application.Current.MainPage.DisplayAlert("Éxito", "Cosecha registrada", "OK");
    }

    async Task CambiarEstado()
    {
        var estados = Enum.GetNames(typeof(EstadoPlanta));
        var resultado = await Application.Current.MainPage.DisplayActionSheet(
            "Cambiar estado", 
            "Cancelar", 
            null, 
            estados);

        if (resultado != "Cancelar" && Enum.TryParse<EstadoPlanta>(resultado, out var nuevoEstado))
        {
            Planta.Estado = nuevoEstado;
            await _context.SaveChangesAsync();
        }
    }

    async Task EditarPlanta()
    {
        await Application.Current.MainPage.DisplayAlert("Info", "Función de edición en desarrollo", "OK");
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
    
    async Task VerHistorialRiegos()
    {
        await Shell.Current.GoToAsync($"HistorialRiegosPage?plantaId={PlantaId}");
    }
    
    void LimpiarFormularioCosecha()
    {
        CantidadCosechaNueva = 0;
        CalidadCosechaNueva = CalidadCosecha.Buena;
        NotasCosechaNueva = string.Empty;
        FotoTemporal = null;
    }
}
