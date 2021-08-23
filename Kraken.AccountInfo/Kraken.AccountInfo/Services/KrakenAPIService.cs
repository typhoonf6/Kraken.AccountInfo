using Kraken.AccountInfo;
using Newtonsoft.Json;
using QuickGraph;
using QuickGraph.Algorithms;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

[assembly: Dependency(typeof(KrakenAPIService))]
namespace Kraken.AccountInfo
{
    /// <summary>
    /// Returns data from the Kraken API
    /// </summary>
    public class KrakenAPIService : IKrakenAPIService
    {
        #region Kraken API strings
        private readonly string APIPath;
        private readonly string privateEndpoint;
        private readonly string publicEndpoint;
        private readonly string mAssetInfo;
        private readonly string mAccountBalance;
        private readonly string mAssetPairs;
        private readonly string mTicker;
        #endregion

        #region Collections
        private AdjacencyGraph<string, Edge<string>> CurrencyGraph;

        private Dictionary<string, Asset> Assets;

        private Dictionary<string, Pair> Pairs;

        private Keys info;
        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public KrakenAPIService()
        {
            APIPath = Application.Current.Resources["apiPath"] as string;
            privateEndpoint = Application.Current.Resources["apiPrivateEndpoint"] as string;
            publicEndpoint = Application.Current.Resources["apiPublicEndpoint"] as string;
            mAssetInfo = Application.Current.Resources["apiMethodAssInf"] as string;
            mAccountBalance = Application.Current.Resources["apiMethodAccBal"] as string;
            mAssetPairs = Application.Current.Resources["apiMethodAssetPairs"] as string;
            mTicker = Application.Current.Resources["apiMethodTicker"] as string;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Does a full pull of all API data to fill the collection initially
        /// </summary>
        /// <returns></returns>
        public async Task<ObservableRangeCollection<Asset>> InitializeDataAsync()
        {
            await init();

            var userAssets = await GetBalanceAsync();

            userAssets = await GetAssetValue(userAssets);

            return userAssets;
        }

        /// <summary>
        /// Refreshes data necessary for recalculation of assets and their value
        /// Must return a full set, as ObservableRangeCollection doesn't proc
        /// if an update is done to an internal property.
        /// </summary>
        /// <param name="assets"></param>
        /// <returns></returns>
        public async Task<ObservableRangeCollection<Asset>> RefreshDataAsync(ObservableRangeCollection<Asset> assets)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the users current holdings from the API
        /// </summary>
        /// <returns>JSON string containing user balance. Should be deserialized.</returns>
        private async Task<ObservableRangeCollection<Asset>> GetBalanceAsync()
        {
            var request = new MyWebRequest(APIPath, privateEndpoint, mAccountBalance);
            var result = await WebRequestHelper.MakeSignedRequestAsync(request, info.PublicKey, info.PrivateKey);
            var dResult = JsonConvert.DeserializeObject<KrakenData<string>>(result).result;

            var userAssets = new ObservableRangeCollection<Asset>();
            foreach (var key in dResult.Keys)
            {
                var userAsset = Assets[key];
                var amount = double.Parse(dResult[key]);
                if (amount < 0.2)
                    continue;
                userAsset.Amount = amount;
                userAssets.Add(userAsset);
            }

            return userAssets;
        }

        /// <summary>
        /// Return a ticker for the asset pair
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        private async Task UpdateTickersAsync(ObservableRangeCollection<Asset> userAssets)
        {
            await init();

            var sb = new StringBuilder("pair=");

            foreach (var asset in userAssets)
            {
                foreach (var leg in asset.ConversionRoute)
                {
                    var tickerName = $"{leg.Source}{leg.Target},";
                    sb.Append(tickerName);
                }
            }

            sb.Remove(sb.Length - 1, 1);
            var request = new MyWebRequest(APIPath, publicEndpoint, mTicker);
            var result = await WebRequestHelper.MakeRequestAsync(request, sb.ToString());
            var tickers = JsonConvert.DeserializeObject<KrakenData<Ticker>>(result).result;
            
            foreach (var ticker in tickers)
            {
                Pairs[ticker.Key].Ticker = ticker.Value;
            }
        }

        /// <summary>
        /// Calculates the value of the asset given it's amount and the currency its to convert to
        /// </summary>
        /// <param name="asset"></param>
        private async Task<ObservableRangeCollection<Asset>> GetAssetValue(ObservableRangeCollection<Asset> userAssets)
        {
            foreach (var asset in userAssets)
            {
                if (asset.ConversionRoute == null)
                    asset.ConversionRoute = GetConversionRoute(asset.altname, "AUD");
            }

            await UpdateTickersAsync(userAssets);

            foreach (var asset in userAssets)
            {
                asset.ConversionRate = 1.0;
                foreach (var leg in asset.ConversionRoute)
                {
                    var key = leg.Source + leg.Target;
                    var askPrice = double.Parse(Pairs[key].Ticker.a[0]);
                    var bidPrice = double.Parse(Pairs[key].Ticker.b[0]);
                    asset.ConversionRate *= (askPrice + bidPrice) / 2.0;
                }
                asset.Value = asset.Amount * asset.ConversionRate;
            }
            return userAssets;
        }

        /// <summary>
        /// Lazy load the service.
        /// </summary>
        private async Task init()
        {
            if (Assets != null || Pairs != null)
                return;

            var db = DependencyService.Get<IDatabaseService>();

            var infRequestTask = WebRequestHelper.MakeRequestAsync(new MyWebRequest(APIPath, publicEndpoint, mAssetInfo));
            var pairRequestTask = WebRequestHelper.MakeRequestAsync(new MyWebRequest(APIPath, publicEndpoint, mAssetPairs));
            var keysTask = db.GetKeysAsync();

            await Task.WhenAll(infRequestTask, pairRequestTask, keysTask);

            var infResult = await infRequestTask;
            var pairResult = await pairRequestTask;

            Assets = JsonConvert.DeserializeObject<KrakenData<Asset>>(infResult).result;
            Pairs = JsonConvert.DeserializeObject<KrakenData<Pair>>(pairResult).result;
            info = await keysTask;
        }
       
        #endregion

        #region Graph

        /// <summary>
        /// Gets all available currency pairs from API and creates a graph.
        /// Graph can then be searched to find conversion rate between assets.
        /// </summary>
        /// <returns>Awaitable Task</returns>
        private void CreateCurrencyGraph()
        {
            if (CurrencyGraph != null)
                return;

            CurrencyGraph = new AdjacencyGraph<string, Edge<string>>();

            // Add pairs to the graph
            foreach (var value in Pairs.Values)
            {
                if (value.AltBase == null | value.AltQuote == null)
                    continue;
                if (!CurrencyGraph.ContainsVertex(value.AltBase))
                    CurrencyGraph.AddVertex(value.AltBase);
                if (!CurrencyGraph.ContainsVertex(value.AltQuote))
                    CurrencyGraph.AddVertex(value.AltQuote);

                CurrencyGraph.AddEdge(new Edge<string>(value.AltBase, value.AltQuote));
                // CurrencyGraph.AddEdge(new Edge<string>(value.AltQuote, value.AltBase));
            }
        }

        /// <summary>
        /// Searches the currency graph to find shortest path to convert root currency to target
        /// </summary>
        /// <param name="root">Name of the root currency</param>
        /// <param name="target">Name of the target currency</param>
        /// <returns></returns>
        private IEnumerable<Edge<string>> GetConversionRoute(string root, string target)
        {
            // Lazy load the graph
            CreateCurrencyGraph();

            Func<Edge<string>, double> edgeCost = edge => 1;

            // Deal with staked assets
            if (root.EndsWith(".S") && !CurrencyGraph.ContainsVertex(root))
                root = root.TrimEnd(new char[] { '.', 'S' });

            TryFunc<string, IEnumerable<Edge<string>>> tryGetPaths = CurrencyGraph.ShortestPathsDijkstra(edgeCost, root);
            _ = tryGetPaths(target, out IEnumerable<Edge<string>> result);
            return result;
        }
        
        #endregion
    }
}
