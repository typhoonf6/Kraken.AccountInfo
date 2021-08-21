using Kraken.AccountInfo;
using Newtonsoft.Json;
using QuickGraph;
using QuickGraph.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly string mTradeBalance;
        private readonly string mAssetPairs;
        private readonly string mTicker;
        #endregion

        private AdjacencyGraph<string, Edge<string>> CurrencyGraph;

        public Dictionary<string, Asset> Assets;

        public Dictionary<string, Pair> Pairs;

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
            mTradeBalance = Application.Current.Resources["apiMethodTraBal"] as string;
            mAssetPairs = Application.Current.Resources["apiMethodAssetPairs"] as string;
            mTicker = Application.Current.Resources["apiMethodTicker"] as string;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Gets the users current holdings from the API
        /// </summary>
        /// <returns>JSON string containing user balance. Should be deserialized.</returns>
        public async Task<ObservableRangeCollection<Asset>> GetBalance()
        {
            await init();

            var request = new MyWebRequest(APIPath, privateEndpoint, mAccountBalance);
            var result = await WebRequestHelper.MakeSignedRequestAsync(request, UserInfo.PublicKey, UserInfo.PrivateKey);
            var assets = JsonConvert.DeserializeObject<KrakenData<string>>(result).result;

            var userAssets = new ObservableRangeCollection<Asset>();
            foreach (var key in assets.Keys)
            {
                var userAsset = Assets[key];
                var amount = double.Parse(assets[key]);
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
        public async Task UpdateTicker(string tickerName)
        {
            await init();

            var arg = $"pair={tickerName}";
            var request = new MyWebRequest(APIPath, publicEndpoint, mTicker);
            var result = await WebRequestHelper.MakeRequestAsync(request, arg);
            var ticker = JsonConvert.DeserializeObject<KrakenData<Ticker>>(result).result;
            
            Pairs[tickerName].Ticker = ticker[tickerName];
        }

        /// <summary>
        /// Calculates the value of the asset given it's amount and the currency its to convert to
        /// </summary>
        /// <param name="asset"></param>
        public async Task GetAssetValue(Asset asset)
        {
            var route = GetConversionRoute(asset.altname, "AUD");

            var conversionRate = 1.0;
            foreach (var leg in route)
            {
                var key = leg.Source + leg.Target;
                await UpdateTicker(key);
                conversionRate *= double.Parse(Pairs[key].Ticker.p[1]);
            }
            asset.Value = asset.Amount * conversionRate;
        }

        /// <summary>
        /// Lazy load the service.
        /// </summary>
        private async Task init()
        {
            if (Assets != null || Pairs != null)
                return;

            var request = new MyWebRequest(APIPath, publicEndpoint, mAssetInfo);
            var result = await WebRequestHelper.MakeRequestAsync(request);
            Assets = JsonConvert.DeserializeObject<KrakenData<Asset>>(result).result;

            request.Method = mAssetPairs;
            result = await WebRequestHelper.MakeRequestAsync(request);
            Pairs = JsonConvert.DeserializeObject<KrakenData<Pair>>(result).result;
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
                CurrencyGraph.AddEdge(new Edge<string>(value.AltQuote, value.AltBase));
            }
        }

        /// <summary>
        /// Searches the currency graph to find shortest path to convert root currency to target
        /// </summary>
        /// <param name="root">Name of the root currency</param>
        /// <param name="target">Name of the target currency</param>
        /// <returns></returns>
        public IEnumerable<Edge<string>> GetConversionRoute(string root, string target)
        {
            CreateCurrencyGraph();

            Func<Edge<string>, double> edgeCost = edge => 1;

            if (root.EndsWith(".S") && !CurrencyGraph.ContainsVertex(root))
                root = root.TrimEnd(new char[] { '.', 'S' });
            try
            {
                TryFunc<string, IEnumerable<Edge<string>>> tryGetPaths = CurrencyGraph.ShortestPathsDijkstra(edgeCost, root);
                _ = tryGetPaths(target, out IEnumerable<Edge<string>> result);
                return result;
            }
            catch (Exception e)
            {
                
                var message = e.Message;
            }
            return null;
        }
        
        #endregion
    }
}
