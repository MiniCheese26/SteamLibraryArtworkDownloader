using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SteamLibraryArtworkDownloader
{
    internal class SteamDownloadManager
    {
        private readonly SteamDownloader _steamDownloader;

        public SteamDownloadManager()
        {
            _steamDownloader = new SteamDownloader();
        }
        
        public async Task RunApp()
        {
            var lookupFilenames = GetLookupFilenames();

            if (lookupFilenames == null)
            {
                Console.WriteLine("Failed to find filenames.txt or failed to read filenames.txt");
                Environment.Exit(1);
            }

            while (true)
            {
                string? appId = string.Empty;

                while (string.IsNullOrWhiteSpace(appId)
                       || !int.TryParse(appId, out _))
                {
                    Console.Write("Enter AppId [n to stop]: ");
                    appId = Console.ReadLine()?.Trim();
                    
                    if (string.Equals(appId, "n", StringComparison.OrdinalIgnoreCase))
                    {
                        _steamDownloader.Dispose();
                        Environment.Exit(0);
                    }
                }

                var tasks = lookupFilenames.Select(lookupFilename => Task.Run(async () =>
                {
                    // ReSharper disable once AccessToDisposedClosure
                    bool downloadResult = await _steamDownloader.Download(appId, lookupFilename);

                    Console.WriteLine(downloadResult
                        ? $"[{appId}] Download of {lookupFilename} successful"
                        : $"[{appId}] Download of {lookupFilename} failed");
                }));

                await Task.WhenAll(tasks);
            }
        }

        private IReadOnlyCollection<string>? GetLookupFilenames()
        {
            string path = Path.Combine(AppContext.BaseDirectory, "filenames.txt");
            
            if (!File.Exists(path))
            {
                return null;
            }
            
            try
            {
                return File.ReadLines(path)
                    .ToList()
                    .AsReadOnly();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
                return null;
            }
        }
    }
}