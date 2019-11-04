using System.Diagnostics;
using System.Threading.Tasks;

namespace SteamLibraryArtworkDownloader
{
    internal static class Program
    {
        private static async Task Main()
        {
            var manager = new SteamDownloadManager();
            await manager.RunApp();
        }
    }
}