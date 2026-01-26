using hustle_garden.Views;

namespace hustle_garden
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            
            Routing.RegisterRoute("DetallePlantaPage", typeof(DetallePlantaPage));
            
            // Escuchar cuando cambia la pestaña seleccionada
            this.PropertyChanged += OnShellPropertyChanged;
        }

        private async void OnShellPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Solo actuar cuando cambia la pestaña actual
            if (e.PropertyName == nameof(CurrentItem))
            {
                // Solo hacer Pop si hay páginas en la pila
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
