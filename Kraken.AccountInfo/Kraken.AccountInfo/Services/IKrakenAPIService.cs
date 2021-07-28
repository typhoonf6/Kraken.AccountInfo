using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Kraken.AccountInfo
{
    /// <summary>
    /// Service contract for dependency injection
    /// </summary>
    public interface IKrakenAPIService<T>
    {
        Task<IEnumerable<T>> GetPublicTickers();
        Task<IEnumerable<T>> GetAccountBalance();
        Task<bool> CheckAccess();
    }
}
