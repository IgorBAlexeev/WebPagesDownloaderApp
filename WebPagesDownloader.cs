using System;
using System.Net;
namespace WebPagesDownloaderApp
{
    public class WebPagesDownloader
    {
        public static async Task<List<Website>> ParallelDownload(List<string> sites, IProgress<ProgressReport> progress, CancellationTokenSource cancellationTokenSource)
        {
            var list = new List<Website>();
            var progressReport = new ProgressReport();
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism  = 8, CancellationToken = cancellationTokenSource.Token };
            await Task.Run(() =>
            {
                try
                {
                    Parallel.ForEach<string>(sites, parallelOptions, (site) =>
                    {
                        var result = DownloadWebPage(site);
                        list.Add(result);
                        progressReport.SitesDownloaded = list;
                        progressReport.PercentageCompleted = (list.Count * 100) / sites.Count;
                        progress.Report(progressReport);
                        parallelOptions.CancellationToken.ThrowIfCancellationRequested();
                    });
                }
                catch (OperationCanceledException ex)
                {
                    throw ex;
                }
            });
            return list;
        }
        private static Website DownloadWebPage(string url)
        {
            var client = new WebClient();
            return new Website { Url = url, Data = client.DownloadString(url) };
        }
    }
    public class Website
    {
        public string Url { get; set; }
        public string Data { get; set; }
    }
    public class ProgressReport
    {
        public int PercentageCompleted { get; set; }
        public List<Website> SitesDownloaded { get; set; }
    }
}
