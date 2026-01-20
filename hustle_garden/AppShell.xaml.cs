using hustle_garden.Views;

namespace hustle_garden
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            
            Routing.RegisterRoute("DetallePlantaPage", typeof(DetallePlantaPage));
        }
    }
}
