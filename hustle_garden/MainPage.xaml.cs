using HuertoApp.ViewModels;

namespace hustle_garden
{
    public partial class MainPage : ContentPage
    {
        public MainPage(HuertoViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}
