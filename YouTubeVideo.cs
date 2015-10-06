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

        public YouTubeVideo(YouTubeURL yturl, YouTubePage ytp, YouTubeCommentsPage ytcp)
        {
            Url = yturl;
            Page = ytp;
            CommentsPage = ytcp;
        }
        public YouTubeVideo(YouTubePage ytp, YouTubeCommentsPage ytcp)
        {
            Url = ytp.VideoUrl;
            Page = ytp;
            CommentsPage = ytcp;
        }
    }
}
