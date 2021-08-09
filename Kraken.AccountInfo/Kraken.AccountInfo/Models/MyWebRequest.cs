using System.Collections.Generic;

namespace Kraken.AccountInfo
{
    /// <summary>
    /// Hold request info to make it less messy
    /// </summary>
    public class MyWebRequest
    {
        public string APIPath { get; set; }
        public string Endpoint { get; set; }
        public string Method { get; set; }
        public List<KeyValuePair<string, string>> PostData { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="apiPath"></param>
        /// <param name="endpoint"></param>
        /// <param name="method"></param>
        public MyWebRequest(string apiPath, string endpoint, string method)
        {
            APIPath = apiPath;
            Endpoint = endpoint;
            Method = method;
        }

        public string URIPath()
        {
            return Endpoint + Method;
        }

        public string FullPath()
        {
            return APIPath + URIPath();
        }
    }
}
