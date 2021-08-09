using QuickGraph;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kraken.AccountInfo
{
    /// <summary>
    /// Service contract for dependency injection
    /// </summary>
    public interface IKrakenAPIService
    {
        Task CreateCurrencyGraph();
        Task<IEnumerable<Edge<string>>> GetConversionRoute(string root, string target);
        Task<string> GetUserHoldings();
    }
}
