using System;
using System.Json;
using System.Net;
using System.Threading.Tasks;
namespace JAM.WebScraper.Android.Helpers
{
    public static class Http
    {
        public static async Task<JsonValue> GetResponse(string url, int timeout = 15000)
        {
            // Create an HTTP web request using the URL:
            var request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.Timeout = timeout;
            request.ContentType = "application/json";
            request.Method = "GET";

            // Send the request to the server and wait for the response:
            using (var response = await request.GetResponseAsync())
            {
                // Get a stream representation of the HTTP web response:
                using (var stream = response.GetResponseStream())
                {
                    // Use this stream to build a JSON document object:
                    var jsonDoc = await Task.Run(() => JsonValue.Load(stream));
                    return jsonDoc;
                }
            }
        }

    }
}