using System;
using System.IO;

namespace SteamLibraryArtworkDownloader
{
    internal class SteamFileOperations
    {
        private readonly Stream _stream;
        private readonly string _appId;
        private readonly string _filename;

        public SteamFileOperations(Stream stream, string appId, string filename)
        {
            _stream = stream;
            _appId = appId;
            _filename = filename;
        }

        public bool WriteImageStream()
        {
            string downloadFolderPath = Path.Combine(AppContext.BaseDirectory, "Downloads", _appId);

            if (!Directory.Exists(downloadFolderPath))
            {
                try
                {
                    Directory.CreateDirectory(downloadFolderPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{_appId}] Failed to create download directory, {ex.Message}");
                    return false;
                }
            }
            
            string downloadFilePath = Path.Combine(downloadFolderPath, _filename);
            using FileStream imageFileStream = File.Create(downloadFilePath);

            try
            {
                _stream.CopyTo(imageFileStream);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{_appId}] Failed to copy image stream to {_filename}, {ex.Message}");
                return false;
            }

            return true;
        }
    }
}