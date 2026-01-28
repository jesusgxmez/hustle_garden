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
        var guardadoExitoso = await _viewModel.GuardarPlantaDesdeModal();
        
        if (guardadoExitoso)
        {
            await Navigation.PopModalAsync();
        }
    }
}
