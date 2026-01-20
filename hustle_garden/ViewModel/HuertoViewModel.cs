using PropertyChanged;
using System.Collections.ObjectModel;
using System.Windows.Input;
using HuertoApp.Models;
using HuertoApp.Data;
using Microsoft.EntityFrameworkCore;

namespace HuertoApp.ViewModels;

[AddINotifyPropertyChangedInterface]
public class HuertoViewModel
{
    private readonly HuertoContext _context;

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

    public HuertoViewModel(HuertoContext context)
    {
        _context = context;
        
        TomarFotoCommand = new Command(async () => await TomarFoto());
        SeleccionarGaleriaCommand = new Command(async () => await SeleccionarDeGaleria());
        GuardarPlantaCommand = new Command(async () => await GuardarPlanta());
        BorrarPlantaCommand = new Command<Planta>(async (p) => await BorrarPlanta(p));
        EditarPlantaCommand = new Command<Planta>(async (p) => await EditarPlanta(p));
        CambiarEstadoPlantaCommand = new Command<Planta>(async (p) => await CambiarEstadoPlanta(p));
        FiltrarPorEstadoCommand = new Command<string>((estado) => FiltrarPlantas(estado));
        VerDetallePlantaCommand = new Command<Planta>(async (p) => await VerDetallePlanta(p));

        CargarPlantas();
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

    async Task GuardarPlanta()
    {
        if (string.IsNullOrWhiteSpace(NombreNuevaPlanta))
        {
            await Application.Current.MainPage.DisplayAlert("Validación", "El nombre de la planta es obligatorio", "OK");
            return;
        }

        var nueva = new Planta
        {
            Nombre = NombreNuevaPlanta,
            Variedad = VariedadNuevaPlanta,
            Ubicacion = UbicacionNuevaPlanta,
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
        
        await Application.Current.MainPage.DisplayAlert("Éxito", "Planta guardada correctamente", "OK");
    }

    async Task EditarPlanta(Planta planta)
    {
        if (planta == null) return;
        
        await Shell.Current.GoToAsync($"DetallePlantaPage?plantaId={planta.Id}");
    }

    async void CargarPlantas()
    {
        var datos = await _context.Plantas
            .Include(p => p.Riegos)
            .Include(p => p.Tareas)
            .Include(p => p.Cosechas)
            .ToListAsync();
            
        Plantas = new ObservableCollection<Planta>(datos);
        PlantasQueSiembraRiego = new ObservableCollection<Planta>(datos.Where(p => p.NecesitaRiego));
    }

    async Task BorrarPlanta(Planta planta)
    {
        if (planta == null) return;

        var confirmar = await Application.Current.MainPage.DisplayAlert(
            "Confirmar", 
            $"¿Estás seguro de eliminar {planta.Nombre}?", 
            "Sí", 
            "No");
            
        if (!confirmar) return;

        _context.Plantas.Remove(planta);
        await _context.SaveChangesAsync();

        Plantas.Remove(planta);
    }
    
    async Task CambiarEstadoPlanta(Planta planta)
    {
        if (planta == null) return;
        
        var estados = Enum.GetNames(typeof(EstadoPlanta));
        var resultado = await Application.Current.MainPage.DisplayActionSheet(
            "Cambiar estado de la planta", 
            "Cancelar", 
            null, 
            estados);
            
        if (resultado != "Cancelar" && Enum.TryParse<EstadoPlanta>(resultado, out var nuevoEstado))
        {
            planta.Estado = nuevoEstado;
            await _context.SaveChangesAsync();
        }
    }
    
    void FiltrarPlantas(string estado)
    {
        FiltroEstado = estado;
        
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