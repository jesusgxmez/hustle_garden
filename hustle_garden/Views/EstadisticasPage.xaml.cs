using HuertoApp.ViewModels;

namespace hustle_garden.Views;

public partial class EstadisticasPage : ContentPage
{
    public EstadisticasPage(EstadisticasViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
