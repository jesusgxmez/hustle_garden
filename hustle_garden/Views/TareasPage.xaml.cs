using HuertoApp.ViewModels;
using HuertoApp.Models;

namespace hustle_garden.Views;

public partial class TareasPage : ContentPage
{
    TareasViewModel viewModel;

    public TareasPage(TareasViewModel vm)
    {
        InitializeComponent();
        BindingContext = viewModel = vm;
    }

    void OnCheckboxChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox checkbox && checkbox.BindingContext is Tarea tarea)
        {
            viewModel.CompletarTareaCommand.Execute(tarea);
        }
    }
}
