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

        public YouTubeCommentsPage(YouTubeUrl ytUrl) : base (ytUrl)
        {
            VideoUrl = ytUrl;
        }
        public async Task DownloadYouTubeCommentsPageAsync()
        {
            await DownloadPageAsync();
        }

        public void GetNumComments()
        {
            var numCommentsMatch = Regex.Match(Page, @"\<strong\>All\sComments\<\/strong\>\s\((?<comments>[^\)]*)");
            _numComments = numCommentsMatch.Groups["comments"].Success ? int.Parse(numCommentsMatch.Groups["comments"].Value, NumberStyles.AllowThousands) : 0;
        }
    }
}
