using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeParse
{
    public class YouTubeVideoEventArgs
    {
        public YouTubeVideoEventArgs(YouTubeVideo video)
        {
            Video = video;
        }
        public YouTubeVideo Video { get; private set; }
    }
    public class YouTubeVideo : IEquatable<YouTubeVideo>
    {
        public YouTubeUrl Url { get; set; }
        public YouTubeVideoPage VideoPage { get; set; }
        public YouTubeCommentsPage CommentsPage { get; set; }
        //public YouTubeVideoThumbnail Thumbnail { get; set; }
        public bool IsVideoReady => VideoPage.IsPageDownloaded && CommentsPage.IsPageDownloaded;

        public YouTubeVideo(YouTubeUrl ytUrl)
        {
            Url = ytUrl;
            VideoPage = new YouTubeVideoPage(ytUrl);
            CommentsPage = new YouTubeCommentsPage(ytUrl);
        }
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

        public async Task DownloadPages()
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
            OnVideoPagesDownloaded(new YouTubeVideoEventArgs(this));
        }

        public event EventHandler<YouTubeVideoEventArgs> VideoPagesDownloaded;

        protected virtual void OnVideoPagesDownloaded(YouTubeVideoEventArgs e)
        {
            VideoPagesDownloaded?.Invoke(this, e);
        }

        #region Equality Members
        public bool Equals(YouTubeVideo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Url, other.Url) && Equals(VideoPage, other.VideoPage) && Equals(CommentsPage, other.CommentsPage);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((YouTubeVideo) obj);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Url != null ? Url.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (VideoPage != null ? VideoPage.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (CommentsPage != null ? CommentsPage.GetHashCode() : 0);
                return hashCode;
            }
        }
        #endregion
    }
}
