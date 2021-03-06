using Xamarin.Forms;

namespace Kraken.AccountInfo
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Page routes
            Routing.RegisterRoute(nameof(PersonalDetailsPage), typeof(PersonalDetailsPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        }
    }
}
