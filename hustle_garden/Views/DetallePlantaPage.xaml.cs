using HuertoApp.ViewModels;
using System;

namespace hustle_garden.Views;

public partial class DetallePlantaPage : ContentPage
{
    DetallePlantaViewModel viewModel;

    public DetallePlantaPage(DetallePlantaViewModel vm)
    {
        InitializeComponent();
        BindingContext = viewModel = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await viewModel.CargarDatos();
    }

    private async void OnCerrarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
