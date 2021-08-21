using QuickGraph;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Kraken.AccountInfo
{
    /// <summary>
    /// Service contract for dependency injection
    /// </summary>
    public interface IKrakenAPIService
    {
        IEnumerable<Edge<string>> GetConversionRoute(string root, string target);
        Task UpdateTicker(string tickerName);
        Task<ObservableRangeCollection<Asset>> GetBalance();
        Task GetAssetValue(Asset asset);
    }
}
