using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeParse
{
    public class YouTubeVideo
    {
        public YouTubeUrl Url { get; set; }
        public YouTubeVideoPage VideoPage { get; set; }
        public YouTubeCommentsPage CommentsPage { get; set; }
        //public YouTubeVideoThumbnail Thumbnail { get; set; }

        public YouTubeVideo(YouTubeUrl ytUrl)
        {
            Url = ytUrl;
            VideoPage = new YouTubeVideoPage(ytUrl);
            CommentsPage = new YouTubeCommentsPage(ytUrl);
            DownloadPages();
        }
        public YouTubeVideo(YouTubeUrl ytUrl, YouTubeVideoPage ytVideoPage, YouTubeCommentsPage ytComPage)
        {
            Url = ytUrl;
            VideoPage = ytVideoPage;
            CommentsPage = ytComPage;
            DownloadPages();
        }
        public YouTubeVideo(YouTubeVideoPage ytVideoPage, YouTubeCommentsPage ytComPage)
        {
            Url = ytVideoPage.VideoUrl;
            VideoPage = ytVideoPage;
            CommentsPage = ytComPage;
            DownloadPages();
        }

        private async void DownloadPages()
        {
            var allPageTypes = new YouTubeHtmlPage[]
            {
                VideoPage, CommentsPage
            };
            var pagesToDownload = from pageType in allPageTypes
                where !pageType.IsPageDownloaded
                select pageType;
            var pagesArray = pagesToDownload.ToArray();

            if (pagesArray.Length < 1) return;

            var tasks = new Task[pagesArray.Length];
            int tasksIndex = 0;
            foreach (var page in pagesArray)
            {
                tasks[tasksIndex] = page.DownloadPageAsync();
                tasksIndex++;
            }

            await Task.WhenAll(tasks);

            //var tasks = new Task[2];
            //tasks[0] = Page.DownloadYouTubePageAsync();
            //tasks[1] = CommentsPage.DownloadYouTubeCommentsPageAsync();
            //await Task.WhenAll(tasks);
        }
    }
}
