using System.Security.Cryptography;
using System.Text;
using System;

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
        /// Signs a message with apiKey
        /// </summary>
        /// <param name="endpoint">The REST endpoint</param>
        /// <param name="postData">The request</param>
        /// <param name="apiPrivateKey"></param>
        /// <returns></returns>
        public static string SignMessage(string endpoint, string postData, string apiPrivateKey)
        {
            // Step 1: concatenate postData + endpoint
            var message = postData + endpoint;

            //Step 2: hash the result of step 1 with SHA256
            var hash256 = new SHA256Managed();
            var hash = hash256.ComputeHash(Encoding.UTF8.GetBytes(message));

            //step 3: base64 decode apiPrivateKey
            var secretDecoded = Convert.FromBase64String(apiPrivateKey);

            //step 4: use result of step 3 to hash the resultof step 2 with HMAC-SHA512
            var hmacsha512 = new HMACSHA512(secretDecoded);
            var hash2 = hmacsha512.ComputeHash(hash);

            //step 5: base64 encode the result of step 4 and return
            return Convert.ToBase64String(hash2);

        }


        /// <summary>
        /// Make the HTTP request
        /// </summary>
        /// <param name="requestMethod">The method of request. This is likely on GET for this application</param>
        /// <param name="endpoint">Endpoint URL of the data request</param>
        /// <param name="postUrl"></param>
        /// <param name="postBody"></param>
        /// <returns></returns>
        public string MakeRequest(string requestMethod, string endpoint, string postUrl = "", string postBody = "")
        {

            /*if (!checkCertificate)
            {
                ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            }
            using (var client = new WebClient())
            {
                var url = apiPath + endpoint + "?" + postUrl;

                //create authentication headers
                if (apiPublicKey != null && apiPrivateKey != null)
                {
                    var postData = postUrl + postBody;
                    var signature = SignMessage(endpoint, postData);
                    client.Headers.Add("APIKey", apiPublicKey);
                    client.Headers.Add("Authent", signature);
                }

                if (requestMethod == "POST" && postBody.Length > 0)
                {
                    NameValueCollection parameters = new NameValueCollection();
                    String[] bodyArray = postBody.Split('&');
                    foreach (String pair in bodyArray)
                    {
                        String[] splitPair = pair.Split('=');
                        parameters.Add(splitPair[0], splitPair[1]);
                    }

                    var response = client.UploadValues(url, "POST", parameters);
                    return Encoding.UTF8.GetString(response);
                }
                else
                {
                    return client.DownloadString(url);
                }
            }*/
            throw new NotImplementedException();
        }
    }
}
