using System.Threading.Tasks;

namespace Kraken.AccountInfo
{
    /// <summary>
    /// Service contract for dependency injection
    /// </summary>
    public interface IDatabaseService
    {
        Task AddKeysAsync(string privateKey, string publicKey);
        Task DeleteKeysAsync();
        Task<Keys> GetKeysAsync();
    }
}