using HuertoApp.ViewModels;

namespace hustle_garden
{
    public partial class MainPage : ContentPage
    {
        HuertoViewModel viewModel;
        public MainPage()
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

    }
}
