using System.Net;
using System.Net.Http;

namespace MyBeatSaberScore.Utility
{
    internal static class HttpTool
    {
        private static HttpClient? _httpClient;

        public static HttpClient Client
        {
            get
            {
                if (_httpClient == null)
                {
                    var handler = new HttpClientHandler()
                    {
                        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                    };
                    _httpClient = new HttpClient(handler);
                }
                return _httpClient;
            }
        }
    }
}
