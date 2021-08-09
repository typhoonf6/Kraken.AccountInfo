using Kraken.AccountInfo;
using Newtonsoft.Json;
using QuickGraph;
using QuickGraph.Algorithms;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private readonly string accountBalance;
        private readonly string tradeBalance;
        private readonly string assetPairs;
        private readonly string ticker;
        #endregion

        private AdjacencyGraph<string, Edge<string>> CurrencyGraph;

        public string PublicKey => "kD7vQhRje/5syCl22cPT153a5hRY7eWBA7GBJVA+Su6qF5ey6Iv5NbuM";

        public string PrivateKey => "Oh37y+b2Ak9FPdPUZU8ho8OIlomFPz4yMQtrGzPemmp0FgXT7ERE2tSLmKi+G1kFspMlp6zurrKC0Uf8Zzfvow==";

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        public KrakenAPIService()
        {
            APIPath = Application.Current.Resources["apiPath"] as string;
            privateEndpoint = Application.Current.Resources["apiPrivateEndpoint"] as string;
            publicEndpoint = Application.Current.Resources["apiPublicEndpoint"] as string;
            accountBalance = Application.Current.Resources["apiMethodAccBal"] as string;
            tradeBalance = Application.Current.Resources["apiMethodTraBal"] as string;
            assetPairs = Application.Current.Resources["apiMethodAssetPairs"] as string;
            ticker = Application.Current.Resources["apiMethodTicker"] as string;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Gets the users current holdings from the API
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetUserHoldings()
        {
            var request = new MyWebRequest(APIPath, privateEndpoint, accountBalance);
            return await WebRequestHelper.MakeSignedRequestAsync(request, PublicKey, PrivateKey);
        }

        /// <summary>
        /// Gets all available currency pairs from API and creates a graph.
        /// Graph can then be searched to find conversion rate between assets.
        /// </summary>
        /// <returns>Awaitable Task</returns>
        public async Task CreateCurrencyGraph()
        {
            if (CurrencyGraph != null)
                return;

            CurrencyGraph = new AdjacencyGraph<string, Edge<string>>();

            // Get the asset pairs
            var request = new MyWebRequest(APIPath, publicEndpoint, assetPairs);
            var requestResult = await WebRequestHelper.MakeRequestAsync(request);
            var assetData = JsonConvert.DeserializeObject<KrakenData<KrakenPair>>(requestResult);

            // Add pairs to the graph
            foreach (var value in assetData.result.Values)
            {
                if (value.Base == null | value.Quote == null)
                    continue;
                if (!CurrencyGraph.ContainsVertex(value.Base))
                    CurrencyGraph.AddVertex(value.Base);
                if (!CurrencyGraph.ContainsVertex(value.Quote))
                    CurrencyGraph.AddVertex(value.Quote);

                CurrencyGraph.AddEdge(new Edge<string>(value.Base, value.Quote));
                CurrencyGraph.AddEdge(new Edge<string>(value.Quote, value.Base));
            }
        }

        /// <summary>
        /// Searches the currency graph to find shortest path to convert root currency to target
        /// </summary>
        /// <param name="root">Name of the root currency</param>
        /// <param name="target">Name of the target currency</param>
        /// <returns></returns>
        public async Task<IEnumerable<Edge<string>>> GetConversionRoute(string root, string target)
        {
            await CreateCurrencyGraph();

            Func<Edge<string>, double> edgeCost = edge => 1;
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

        public async Task<double> GetAssetValue(IEnumerable<Edge<string>> route)
        {
            var request = new MyWebRequest(APIPath, publicEndpoint, 
            var result = WebRequestHelper.MakeRequestAsync();
        }

        #endregion
    }
}
