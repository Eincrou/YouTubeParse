using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YouTubeParse
{
    public class YouTubeCommentsPage : YouTubeHtmlPage
    {
        public YouTubeUrl VideoUrl { get; set; }

        public int NumComments
        {
            get
            {
                if (!_numComments.HasValue)
                    GetNumComments();
                if (_numComments != null) return _numComments.Value;
                throw new Exception("Number of comments could not be determined.");
            }
        }
        private int? _numComments;

        public YouTubeCommentsPage(YouTubeUrl ytUrl) : base (ytUrl.AllCommentsUri.AbsoluteUri)
        {
            VideoUrl = ytUrl;
        }

        public YouTubeCommentsPage(string url) : base (url)
        {
            if(!ValidateYouTubeCommentsPageUrl(url))
                throw new ArgumentException("Invalid YouTube comments page Url", nameof(url));
        }
        /// <summary>
        /// Downloads the HTML of a YouTube comments page. This is required before accessing any information.
        /// </summary>
        public override async Task DownloadPageAsync()
        {
            await base.DownloadPageAsync();
        }

        public static bool ValidateYouTubeCommentsPageUrl(string url)
        {       // https://www.youtube.com/all_comments?v=xxxxxxxxxxx
            if ((url.Contains(@"youtube.com") && url.Contains(@"all_comments")) && url.Contains(@"v="))
                return true;
            return false;
        }
        private async void GetNumComments()
        {
            if (!IsPageDownloaded) await DownloadPageAsync();
            var numCommentsMatch = Regex.Match(Page, @"\<strong\>All\sComments\<\/strong\>\s\((?<comments>[^\)]*)");
            _numComments = numCommentsMatch.Groups["comments"].Success ? int.Parse(numCommentsMatch.Groups["comments"].Value, NumberStyles.AllowThousands) : 0;
        }
    }
}
