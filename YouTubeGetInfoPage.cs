using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeParse
{
    class YouTubeGetInfoPage : YouTubeHtmlPage
    {
        public YouTubeGetInfoPage( YouTubeUrl yturl) : base(yturl.VideoInfoUri.AbsoluteUri)
        {
        }
        public YouTubeGetInfoPage(string url) : base(url)
        {
        }

        public static bool ValidateYouTubeGetInfoUrl(string url)
        {
            // http://youtube.com/get_video_info?video_id=xxxxxxxxxxx
            return url.Contains("youtube.com") && url.Contains("get_video_info") && url.Contains("video_id");
        }
    }
}
