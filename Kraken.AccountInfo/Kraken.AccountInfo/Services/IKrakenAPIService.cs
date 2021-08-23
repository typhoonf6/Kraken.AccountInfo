using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Kraken.AccountInfo
{
    /// <summary>
    /// Service contract for dependency injection
    /// </summary>
    public interface IKrakenAPIService
    {
        Task<ObservableRangeCollection<Asset>> InitializeDataAsync();
        Task<ObservableRangeCollection<Asset>> RefreshDataAsync(ObservableRangeCollection<Asset> assets);
    }
}
