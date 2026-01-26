using HuertoApp.ViewModels;
using HuertoApp.Models;

namespace hustle_garden.Views;

public partial class TareasPage : ContentPage
{
    TareasViewModel viewModel;
    private bool _isUpdating = false;

    public TareasPage(TareasViewModel vm)
    {
        InitializeComponent();
        BindingContext = viewModel = vm;
    }

    void OnCheckboxChanged(object sender, CheckedChangedEventArgs e)
    {
        if (_isUpdating) return;
        
        _isUpdating = true;
        
        try
        {
            if (sender is CheckBox checkbox && checkbox.BindingContext is Tarea tarea)
            {
                // Ejecutar el comando de forma asíncrona sin esperar
                Task.Run(async () =>
                {
                    await Task.Run(() => viewModel.CompletarTareaCommand.Execute(tarea));
                });
            }
        }
        finally
        {
            // Esperar un poco antes de permitir otro cambio
            Task.Delay(100).ContinueWith(_ => _isUpdating = false);
        }
    }
}
