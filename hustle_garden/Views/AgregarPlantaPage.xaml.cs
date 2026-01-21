using HuertoApp.ViewModels;

namespace hustle_garden.Views;

public partial class AgregarPlantaPage : ContentPage
{
    private readonly HuertoViewModel _viewModel;

    public AgregarPlantaPage(HuertoViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    private async void OnCerrarClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        await _viewModel.GuardarPlantaDesdeModal();
        
        // Verificar si se guardó exitosamente (la última planta tiene el nombre ingresado)
        if (_viewModel.Plantas.Any() && 
            _viewModel.Plantas.FirstOrDefault()?.Nombre == _viewModel.NombreNuevaPlanta?.Trim())
        {
            await Navigation.PopModalAsync();
        }
    }
}
