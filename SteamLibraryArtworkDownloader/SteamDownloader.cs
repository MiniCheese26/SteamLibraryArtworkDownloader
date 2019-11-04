using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SteamLibraryArtworkDownloader
{
    internal class SteamDownloader : IDisposable
    {
        private readonly HttpClient _httpClient;

        public SteamDownloader()
        {
            _httpClient = new HttpClient();
        }

        public async Task<bool> Download(string appId, string filename)
        {
            string path = $"{appId}/{filename}";

            try
            {
                using HttpResponseMessage imageResponse = await _httpClient.GetAsync(
                    $"https://steamcdn-a.akamaihd.net/steam/apps/{path}");

                if (!imageResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine(
                        $"[{appId}] Failed to successfully make a request to https://steamcdn-a.akamaihd.net/steam/apps{path}, {imageResponse.ReasonPhrase}");
                    return false;
                }

                Stream imageStream = await imageResponse.Content.ReadAsStreamAsync();

                if (imageStream.Length <= 0)
                {
                    Console.WriteLine($"[{appId}] Downloaded image stream for {filename} was empty");
                    return false;
                }

                var fileOperations = new SteamFileOperations(imageStream, appId, filename);
                return fileOperations.WriteImageStream();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"[{appId}] Failed to make HTTP request for {filename}, {ex.Message}");
                return false;
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}