using Kraken.AccountInfo;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(KrakenAPIService))]
namespace Kraken.AccountInfo
{
    /// <summary>
    /// Returns data from the Kraken API
    /// </summary>
    public class KrakenAPIService : IKrakenAPIService<object>
    {
        /// <summary>
        /// Sends a signed message to see if private data access
        /// is granted
        /// </summary>
        /// <returns>Bool indicating success</returns>
        public Task<bool> CheckAccess()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Returns list of all assets owned by user
        /// </summary>
        /// <returns>An array of objects</returns>
        public Task<IEnumerable<object>> GetAccountBalance()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Returns a list of all public assets
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<object>> GetPublicTickers()
        {
            throw new System.NotImplementedException();
        }
    }
}
