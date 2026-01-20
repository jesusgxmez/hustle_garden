using HuertoApp.ViewModels;

namespace hustle_garden.Views;

public partial class NotasPage : ContentPage
{
    public NotasPage(NotasViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
