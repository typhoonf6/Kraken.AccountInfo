using Kraken.AccountInfo;
using SQLite;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(DatabaseService))]
namespace Kraken.AccountInfo
{
    public class DatabaseService : IDatabaseService
    {
        /// <summary>
        /// Database connection
        /// </summary>
        private SQLiteAsyncConnection db;

        /// <summary>
        /// Adds the keys to the database
        /// </summary>
        /// <param name="privateKey">The private key for the api</param>
        /// <param name="publicKey">public key for the api</param>
        /// <returns></returns>
        public async Task AddKeysAsync(string privateKey, string publicKey)
        {
            await init();

            // TODO Check there isn't already an entry
            var info = new Keys { PrivateKey = privateKey, PublicKey = publicKey };

            await db.InsertAsync(info);
        }

        /// <summary>
        /// Gets the UserInfo containing api keys from the database
        /// </summary>
        /// <returns></returns>
        public async Task<Keys> GetKeysAsync()
        {
            await init();

            var keys = await db.QueryAsync<Keys>("SELECT * FROM [Keys] ORDER BY ROWID ASC LIMIT 1");

            if (keys.Count > 0)
                return keys[0] as Keys;

            return new Keys();
        }

        /// <summary>
        /// Deletes everything in the table. Ensures the next entry is the only one.
        /// </summary>
        /// <returns></returns>
        public async Task DeleteKeysAsync()
        {
            await init();

            await db.DeleteAllAsync<Keys>();
        }

        /// <summary>
        /// Creates the database or just gets the connection if it exists
        /// </summary>
        /// <returns></returns>
        private async Task init()
        {
            if (db != null)
                return;

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "keys.db");

            db = new SQLiteAsyncConnection(dbPath);

            _ = await db.CreateTableAsync<Keys>();
        }
    }
}
