using HuertoApp.Data;
using HuertoApp.ViewModels;
using Microsoft.Extensions.Logging;

namespace hustle_garden
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>();

            // Registro del Contexto de EF Core
            builder.Services.AddDbContext<HuertoContext>();

            // ViewModels y Páginas
            builder.Services.AddTransient<HuertoViewModel>();
            builder.Services.AddTransient<MainPage>();

            return builder.Build();
        }
    }
}
