using System.Security.Cryptography;
using System.Text;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
using System.Collections.Generic;

namespace Kraken.AccountInfo
{
    /// <summary>
    /// Contains methods for making HTTP webrequests and serializing data with JSON
    /// Most of the code is a derivative of that provided at
    /// https://github.com/CryptoFacilities/REST-v3-CSharp/blob/master/C%23/cfRestApiV3/cfRestApiV3/CfApiMethods.cs
    /// </summary>
    public class WebRequestHelper
    {
        /// <summary>
        /// Static http client to use for life of application
        /// </summary>
        private static HttpClient _httpClient;

        /// <summary>
        /// Makes a signed request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="endpoint"></param>
        /// <param name="publicKey"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public static async Task<string> MakeSignedRequestAsync(MyWebRequest request, string publicKey, 
            string privateKey, List<KeyValuePair<string, string>> postData = null)
        {
            init();
            var nonce = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            if (postData != null)
                postData.Add(new KeyValuePair<string, string>("nonce", nonce.ToString()));
            else
                postData = new List<KeyValuePair<string, string>> { 
                    new KeyValuePair<string, string>("nonce", nonce.ToString()) 
                };

            var urlEncoded = new FormUrlEncodedContent(postData);
            var message = $"{nonce}{await urlEncoded.ReadAsStringAsync()}";

            var signature = SignMessage(request.URIPath(), message, privateKey);
            _httpClient.DefaultRequestHeaders.Add("API-Key", publicKey);
            _httpClient.DefaultRequestHeaders.Add("API-Sign", signature);

            var response = await _httpClient.PostAsync(request.FullPath(), urlEncoded);

            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Make the HTTP request
        /// </summary>
        /// <param name="endpoint">Endpoint URL of the data request</param>
        /// <param name="postUrl"></param>
        /// <param name="postBody"></param>
        /// <returns></returns>
        public static async Task<string> MakeRequestAsync(MyWebRequest request, string param = null)
        {
            init();
            string fullPath = request.FullPath();
            if (param != null)
                fullPath = $"{fullPath}?{param}";
            var response = await _httpClient.GetAsync(fullPath);
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Signs a message using Kraken specification
        /// </summary>
        /// <param name="endpoint">The REST endpoint</param>
        /// <param name="privateKey">Private key generated at kraken website</param>
        /// <param name="nonce">Uniquely generated integer as per kraken api requirements</param>
        /// <returns></returns>
        public static string SignMessage(string endpoint, string message, string privateKey)
        {
            using (var hash256 = new SHA256Managed())
            using (var hmacsha512 = new HMACSHA512(Convert.FromBase64String(privateKey)))
            {
                var messageHash = hash256.ComputeHash(Encoding.UTF8.GetBytes(message));

                var hash = Encoding.UTF8.GetBytes(endpoint).Concat(messageHash);

                var hash2 = hmacsha512.ComputeHash(hash.ToArray());

                return Convert.ToBase64String(hash2);
            }
        }

        /// <summary>
        /// Initilizes the <see cref="HttpClient"/> if null,
        /// or just clears the headers from the last time it was used.
        /// </summary>
        private static void init()
        {
            if (_httpClient != null)
            {
                _httpClient.DefaultRequestHeaders.Clear();
                return;
            }
            _httpClient = new HttpClient();
        }
    }
}
