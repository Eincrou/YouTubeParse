using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YouTubeParse
{
    public class YouTubeCommentsPage
    {
        private string _page;
        public YouTubeURL VideoUrl { get; set; }

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

        public YouTubeCommentsPage(YouTubeURL yturl)
        {
            VideoUrl = yturl;
        }
        public async Task DownloadYouTubeCommentsPageAsync()
        {
            var downloader = new HttpDownloader(VideoUrl.AllCommentsUri.AbsoluteUri, String.Empty, String.Empty);
            _page = await downloader.GetPageAsync();
        }

        public void GetNumComments()
        {
            var numCommentsMatch = Regex.Match(_page, @"\<strong\>All\sComments\<\/strong\>\s\((?<comments>[^\)]*)");
            _numComments = numCommentsMatch.Groups["comments"].Success ? int.Parse(numCommentsMatch.Groups["comments"].Value, NumberStyles.AllowThousands) : 0;
        }
    }
}
