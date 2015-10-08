using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeParse
{
    public class YouTubeHtmlPage
    {
        protected string Page;
        public bool IsPageDownloaded { get; protected set; }
        public Uri PageUri { get; }

        public YouTubeHtmlPage(YouTubeUrl ytUrl)
        {
            PageUri = ytUrl.LongYTURL;
        }

        public YouTubeHtmlPage(string url)
        {
            if (YouTubeUrl.ValidateUrl(url) || YouTubePlaylistPage.ValidatePlaylistUrl(url)
                || YouTubeChannelPage.ValidateChannelUrl(url))
            {
                PageUri = new Uri(url);
            }
            else
                throw new ArgumentException("URL could not be validated as a supported YouTube page type");
        }
        protected async Task DownloadPageAsync()
        {
            var downloader = new HttpDownloader(PageUri.AbsoluteUri, String.Empty, String.Empty);
            Page = await downloader.GetPageAsync();
            IsPageDownloaded = true;
        }
    }
}
