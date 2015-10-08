using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeParse
{
    public class YouTubeVideo
    {
        public YouTubeURL Url { get; set; }
        public YouTubePage Page { get; set; }
        public YouTubeCommentsPage CommentsPage { get; set; }
        //public YouTubeVideoThumbnail Thumbnail { get; set; }

        //public YouTubeVideo(YouTubeURL ytUrl)
        //{
        //    Url = ytUrl;
        //    Page = new YouTubePage(ytUrl);
        //    CommentsPage = new YouTubeCommentsPage(ytUrl);
        //    DownloadPages();
        //}
        public YouTubeVideo(YouTubeURL ytUrl, YouTubePage ytPage, YouTubeCommentsPage ytComPage)
        {
            Url = ytUrl;
            Page = ytPage;
            CommentsPage = ytComPage;
        }
        public YouTubeVideo(YouTubePage ytPage, YouTubeCommentsPage ytComPage)
        {
            Url = ytPage.VideoUrl;
            Page = ytPage;
            CommentsPage = ytComPage;
        }

        //private async void DownloadPages()
        //{
            
        //    var pagesToDownload = 
        //    var tasks = new Task[2];
        //    tasks[0] = Page.DownloadYouTubePageAsync();
        //    tasks[1] = CommentsPage.DownloadYouTubeCommentsPageAsync();
        //    await Task.WhenAll(tasks);
        //}
    }
}
