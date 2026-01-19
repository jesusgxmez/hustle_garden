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
    public string NombreNuevaPlanta { get; set; }
    public string FotoTemporal { get; set; }

    public ICommand BorrarPlantaCommand { get; }
    public ICommand TomarFotoCommand { get; }
    public ICommand GuardarPlantaCommand { get; }

    public HuertoViewModel(HuertoContext context)
    {
        _context = context;
        TomarFotoCommand = new Command(async () => await TomarFoto());
        GuardarPlantaCommand = new Command(async () => await GuardarPlanta());
        BorrarPlantaCommand = new Command<Planta>(async (p) => await BorrarPlanta(p));

        CargarPlantas();
    }

    async Task TomarFoto()
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

    async Task GuardarPlanta()
    {
        if (string.IsNullOrWhiteSpace(NombreNuevaPlanta)) return;

        var nueva = new Planta
        {
            Nombre = NombreNuevaPlanta,
            FotoPath = FotoTemporal,
            FechaSiembra = DateTime.Now
        };

        _context.Plantas.Add(nueva);
        await _context.SaveChangesAsync();

        Plantas.Add(nueva); // Actualiza la lista en pantalla

        NombreNuevaPlanta = string.Empty;
        FotoTemporal = null;
    }

    async void CargarPlantas()
    {
        var datos = await _context.Plantas.ToListAsync();
        Plantas = new ObservableCollection<Planta>(datos);
    }

    async Task BorrarPlanta(Planta planta)
    {
        if (planta == null) return;

        _context.Plantas.Remove(planta); // Borra de EF Core
        await _context.SaveChangesAsync(); // Guarda en SQLite

        Plantas.Remove(planta); // Borra de la lista visual
    }

}