using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeParse
{
    public class YouTubeVideoGroup
    {
        public IEnumerable<YouTubeVideo> VideoGroupList { get; }

        public int Count { get { return VideoGroupList.Count(); } }
        public List<string> TitlesList
        {
            get { return VideoGroupList.Select(video => video.VideoPage.VideoTitle).ToList(); }
        }
        public List<Uri> UrisList
        {
            get { return VideoGroupList.Select(video => video.Url.LongYTURL).ToList(); }
        }
        //public List<YouTubeVideoThumbnail> ThumbnailsList { get; set; }

        public int ViewsTotal
        {
            get { return VideoGroupList.Sum(video => video.VideoPage.ViewCount); }
        }
        public double ViewsAvg
        {
            get { return VideoGroupList.Average(video => video.VideoPage.ViewCount); }
        }
        public int CommentsTotal
        {
            get { return VideoGroupList.Sum(video => video.CommentsPage.NumComments); }
        }
        public double CommentsAvg
        {
            get { return VideoGroupList.Average(video => video.CommentsPage.NumComments); }
        }
        public TimeSpan DurationTotal
        {
            get { return new TimeSpan(0, 0, 0, 
                (int) VideoGroupList.Sum(video => video.VideoPage.Duration.TotalSeconds)); }
        }
        public TimeSpan DurationAvg
        {
            get {
                return new TimeSpan(0, 0, 0,
                    (int) VideoGroupList.Average(video => video.VideoPage.Duration.TotalSeconds)); }
        }

        public YouTubeVideoGroup(IEnumerable<YouTubeVideo> videosList)
        {
            VideoGroupList = videosList;
        }

        //TODO methods to add/remove videos
    }
}
