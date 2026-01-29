using HuertoApp.Data;
using HuertoApp.ViewModels;
using HuertoApp.Services;
using hustle_garden.Views;
using Microsoft.Extensions.Logging;

namespace hustle_garden
{
    /// <summary>
    /// Clase de configuración de la aplicación MAUI.
    /// Configura servicios de dependencia, ViewModels y páginas.
    /// </summary>
    public static class MauiProgram
    {
        /// <summary>
        /// Crea y configura la aplicación MAUI.
        /// </summary>
        /// <returns>Aplicación MAUI configurada.</returns>
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>();

            builder.Services.AddDbContext<HuertoContext>();
            builder.Services.AddSingleton<IValidationService, ValidationService>();
            builder.Services.AddSingleton<IImageService, ImageService>();

            builder.Services.AddTransient<HuertoViewModel>();
            builder.Services.AddTransient<DetallePlantaViewModel>();
            builder.Services.AddTransient<TareasViewModel>();
            builder.Services.AddTransient<EstadisticasViewModel>();
            builder.Services.AddTransient<NotasViewModel>();

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<AgregarPlantaPage>();
            builder.Services.AddTransient<DetallePlantaPage>();
            builder.Services.AddTransient<TareasPage>();
            builder.Services.AddTransient<EstadisticasPage>();
            builder.Services.AddTransient<NotasPage>();

            return builder.Build();
        }
    }
}
