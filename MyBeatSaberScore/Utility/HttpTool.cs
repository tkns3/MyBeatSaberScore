using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;

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

        public static async Task<T> GetAndDeserialize<T>(string url)
        {
            var res = await HttpTool.Client.GetAsync(url);
            if (!res.IsSuccessStatusCode)
            {
                throw new Exception($"http status is {res.StatusCode}");
            }
            using var httpStream = await res.Content.ReadAsStreamAsync();
            using var sr = new StreamReader(httpStream);
            using var reader = new JsonTextReader(sr);
            var serializer = new JsonSerializer();
            var result = serializer.Deserialize<T>(reader);
            if (result == null)
            {
                throw new Exception("deserialize result is null");
            }
            return result;
        }

        public static async Task<T> PostAndDeserialize<T>(string url, HttpContent? content)
        {
            var res = await HttpTool.Client.PostAsync(url, content);
            if (!res.IsSuccessStatusCode)
            {
                throw new Exception($"http status is {res.StatusCode}");
            }
            using var httpStream = await res.Content.ReadAsStreamAsync();
            using var sr = new StreamReader(httpStream);
            using var reader = new JsonTextReader(sr);
            var serializer = new JsonSerializer();
            var result = serializer.Deserialize<T>(reader);
            if (result == null)
            {
                throw new Exception("deserialize result is null");
            }
            return result;
        }

        public static async Task<(HttpResponseMessage, T?)> DownloadZipAndDeserialize<T>(string url, string entryName, string? etag = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            if (etag != null) request.Headers.Add("If-None-Match", etag);
            var res = await HttpTool.Client.SendAsync(request);
            if (res.StatusCode == HttpStatusCode.NotModified)
            {
                return (res, default(T));
            }
            if (!res.IsSuccessStatusCode)
            {
                throw new Exception($"http status is {res.StatusCode}");
            }
            using var httpStream = await res.Content.ReadAsStreamAsync();
            using var zipArchive = new ZipArchive(httpStream, ZipArchiveMode.Read);
            var entry = zipArchive.GetEntry(entryName);
            if (entry == null)
            {
                throw new Exception($"zip don't contain \"{entryName}\"");
            }
            using var stream = entry.Open();
            using var sr = new StreamReader(stream);
            using var reader = new JsonTextReader(sr);
            var serializer = new JsonSerializer();
            var result = serializer.Deserialize<T>(reader);
            if (result == null)
            {
                throw new Exception("deserialize result is null");
            }
            return (res, result);
        }

        public static async Task<HttpResponseMessage> Download(string url, string output)
        {
            HttpResponseMessage res = await HttpTool.Client.GetAsync(url);
            if (res.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"http status is {res.StatusCode}");
            }
            using var fileStream = new FileStream(output, FileMode.OpenOrCreate, FileAccess.Write);
            using var httpStream = await res.Content.ReadAsStreamAsync();
            await httpStream.CopyToAsync(fileStream);
            return res;
        }
    }
}
