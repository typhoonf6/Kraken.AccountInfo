using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Kraken.AccountInfo
{
    /// <summary>
    /// ViewModel for displaying list of coins associated with
    /// personal account
    /// </summary>
    public class PersonalCoinsViewModel : BaseViewModel
    {
        /// <summary>
        /// A collection of the users assets to display on the UI
        /// </summary>
        public ObservableRangeCollection<Asset> UserAssets { get; set; }

        /// <summary>
        /// Command initiated when the page needs refreshing
        /// </summary>
        public AsyncCommand RefreshCommand { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public PersonalCoinsViewModel()
        {
            UserAssets = new ObservableRangeCollection<Asset>();
            RefreshCommand = new AsyncCommand(GetData);
        }

        /// <summary>
        /// Sends a signed web request to the API to retrieve the users private data
        /// </summary>
        /// <returns></returns>
        private async Task GetData()
        {
            UserAssets = await KrakenService.InitializeDataAsync();
            IsBusy = false;
        }
    }
}
