using Newtonsoft.Json;
using System.Collections.Generic;
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
        public ObservableRangeCollection<Asset> Assets { get; set; }

        /// <summary>
        /// Command initiated when the page needs refreshing
        /// </summary>
        public AsyncCommand RefreshCommand { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public PersonalCoinsViewModel()
        {
            RefreshCommand = new AsyncCommand(GetPrivateData);
            Assets = new ObservableRangeCollection<Asset>();
        }

        /// <summary>
        /// Sends a signed web request to the API to retrieve the users private data
        /// </summary>
        /// <returns></returns>
        private async Task GetPrivateData()
        {
            var result = await KrakenService.GetUserHoldings();
            var assetData = JsonConvert.DeserializeObject<KrakenData<string>>(result);
            var routes = new Dictionary<string, string>();

            foreach (var asset in assetData.result)
            {
                var amount = double.Parse(asset.Value);
                if (amount == 0 || asset.Key == "ZUSD")
                    continue;
                Assets.Add(new Asset(asset.Key, amount));
                var conversionRoute = await KrakenService.GetConversionRoute(asset.Key, "ZAUD");

                // Just make an individual call for each route
            }
            IsBusy = false;
        }
    }
}
