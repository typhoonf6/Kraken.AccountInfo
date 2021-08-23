using Xamarin.Forms;

namespace Kraken.AccountInfo
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            // Registster the service dependency
            DependencyService.Register<KrakenAPIService>();
            DependencyService.Register<DatabaseService>();

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
