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

        //public YouTubeVideo(YouTubeURL ytUrl)
        //{
        //    Url = ytUrl;
        //    Page = new YouTubePage(ytUrl);
        //    CommentsPage = new YouTubeCommentsPage(ytUrl);
        //    DownloadPages();
        //}
        public YouTubeVideo(YouTubeUrl ytUrl, YouTubeVideoPage ytVideoPage, YouTubeCommentsPage ytComPage)
        {
            Url = ytUrl;
            VideoPage = ytVideoPage;
            CommentsPage = ytComPage;
        }
        public YouTubeVideo(YouTubeVideoPage ytVideoPage, YouTubeCommentsPage ytComPage)
        {
            Url = ytVideoPage.VideoUrl;
            VideoPage = ytVideoPage;
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
