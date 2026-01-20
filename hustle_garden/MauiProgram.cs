using HuertoApp.Data;
using HuertoApp.ViewModels;
using hustle_garden.Views;
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

            builder.Services.AddDbContext<HuertoContext>();

            builder.Services.AddTransient<HuertoViewModel>();
            builder.Services.AddTransient<DetallePlantaViewModel>();
            builder.Services.AddTransient<TareasViewModel>();
            builder.Services.AddTransient<EstadisticasViewModel>();
            builder.Services.AddTransient<NotasViewModel>();

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<DetallePlantaPage>();
            builder.Services.AddTransient<TareasPage>();
            builder.Services.AddTransient<EstadisticasPage>();
            builder.Services.AddTransient<NotasPage>();

            return builder.Build();
        }
    }
}
