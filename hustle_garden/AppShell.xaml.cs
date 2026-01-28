using hustle_garden.Views;

namespace hustle_garden
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            
            Routing.RegisterRoute("DetallePlantaPage", typeof(DetallePlantaPage));
            
            this.PropertyChanged += OnShellPropertyChanged;
        }

        private async void OnShellPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CurrentItem))
            {
                if (Navigation.NavigationStack.Count > 1)
                {
                    await Navigation.PopAsync();
                }
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            this.PropertyChanged -= OnShellPropertyChanged;
        }
    }
}
