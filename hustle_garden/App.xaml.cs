using Microsoft.Extensions.DependencyInjection;

namespace hustle_garden
{
    /// <summary>
    /// Clase principal de la aplicación.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Inicializa una nueva instancia de la aplicación.
        /// </summary>
        public App()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Crea la ventana principal de la aplicación.
        /// </summary>
        /// <param name="activationState">Estado de activación.</param>
        /// <returns>Ventana principal con AppShell.</returns>
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}