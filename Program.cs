using System;
using System.Threading;

namespace WebPagesDownloaderApp
{
    internal class Program
    {
        private static CancellationTokenSource? cancellationTokenSource;
        static async Task Main()
        {
            try
            {
                cancellationTokenSource = new CancellationTokenSource();
                var progress = new Progress<ProgressReport>();
                var sites = new List<string> { "https://www.bbc.com/", "https://www.google.com/" };
                var results = await WebPagesDownloader.ParallelDownload(sites, progress, cancellationTokenSource);
                var directory = @"c:\tmp2\pages\";
                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
                for (int i = 0; i < results.Count; i++)
                {
                    File.WriteAllText(directory + CreateFileName(results[i].Url), results[i].Data);
                }
                Console.WriteLine($"All the web pages are in {directory} folder");
            }
            catch (Exception ex)
            {
                // Todo: add logging
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }
        private static string CreateFileName(string url)
        {
            var index = url.IndexOf("//");
            if (index != -1)
            {
                url = url.Substring(index + 2);
            }
            url = url.Replace("/", "");
            url += ".html";
            return url;
        }
    }
}