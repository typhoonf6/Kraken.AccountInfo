using PropertyChanged;
using System.ComponentModel;
using Xamarin.Forms;

namespace Kraken.AccountInfo
{
    /// <summary>
    /// Adds the foddy weaver package. This automatically implements the PropertyChanged
    /// method on any Properties within the class
    /// </summary>
    [AddINotifyPropertyChangedInterface]

    /// <summary>
    /// Base view model that fires property event changes
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The event that is fired is when any child property changes its value
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        /// <summary>
        /// Grabs the service dependency to access the Kraken API
        /// </summary>
        public IKrakenAPIService KrakenService => DependencyService.Get<IKrakenAPIService>();

        /// <summary>
        /// Standard IsBusy flag
        /// </summary>
        public bool IsBusy { get; set; }
    }
}

