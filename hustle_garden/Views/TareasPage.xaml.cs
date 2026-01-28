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

    protected override async void OnAppearing()
    {
        base.OnAppearing();
     
        await viewModel.RecargarPlantas();
    }

    void OnCheckboxChanged(object sender, CheckedChangedEventArgs e)
    {
        if (_isUpdating) return;
        
        _isUpdating = true;
        
        try
        {
            if (sender is CheckBox checkbox && checkbox.BindingContext is Tarea tarea)
            {
                Task.Run(async () =>
                {
                    await Task.Run(() => viewModel.CompletarTareaCommand.Execute(tarea));
                });
            }
        }
        finally
        {
            Task.Delay(100).ContinueWith(_ => _isUpdating = false);
        }
    }
}
