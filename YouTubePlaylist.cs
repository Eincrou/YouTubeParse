using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YouTubeParse
{
    public class YouTubePlaylist
    {
        private readonly string _playlistId;
        private readonly string _playlistPage;
        private readonly List<YouTubeURL> _pageUrls = new List<YouTubeURL>();
        /// <summary>
        /// URL to this playlist's main page
        /// </summary>
        public string PlaylistUrl
        {
            get { return @"https://www.youtube.com/playlist?list=" + _playlistId; } 
        }
        /// <summary>
        /// Title of this playlist
        /// </summary>
        public string Title { get; private set; }
        /// <summary>
        /// Name of the channel that created and owns this playlist
        /// </summary>
        public string Owner { get; private set; }
        /// <summary>
        /// Number of videos in this playlist
        /// </summary>
        public int NumVideos { get; private set; }
        /// <summary>
        /// Number of page views for this playlist
        /// </summary>
        public int Views { get; private set; }
        /// <summary>
        /// Date of the last time the playlist owner changed this playlist
        /// </summary>
        public DateTime LastUpdated { get; private set; }
        /// <summary>
        /// Total duration of all videos in this playlist
        /// </summary>
        public TimeSpan Duration { get; private set; }
        /// <summary>
        /// All of the videos in this playlist
        /// </summary>
        public List<YouTubeURL> VideoUrlsList { get { return _pageUrls; } }

        /// <summary>
        /// Instantiates a new YouTubePlaylist object, containing information about a playlist on YouTube.
        /// </summary>
        /// <param name="playlistUrl">String representing a valid YouTube playlist URL</param>
        public YouTubePlaylist(string playlistUrl)
        {
            if (!ValidatePlaylistUrl(playlistUrl))
                throw new ArgumentException("Invalid YouTube playlist URL", "playlistUrl");
            _playlistId = GetPlaylistId(playlistUrl);
            var downloader = new HttpDownloader(PlaylistUrl, String.Empty, String.Empty);
            _playlistPage = downloader.GetPage();

            GetPlaylistInformation();
            GetAllYouTubeUrls();
            GetTotalDuration();
        }

        /// <summary>
        /// Checks if a string has YouTube playlist information
        /// </summary>
        /// <param name="url">String to validate as continaing a YouTube playlist</param>
        /// <returns>Whether the input string is a URL with a valid YouTube playlist</returns>
        public static bool ValidatePlaylistUrl(string url)
        {
            if ((url.Contains("youtube.com") || url.Contains("youtu.be")) && url.Contains("list="))
                return true;
            return false;
        }

        public YouTubeVideoGroup GetYouTubeVideoGroup()
        {
            List<YouTubeVideo> videosList = new List<YouTubeVideo>();
            foreach (var video in VideoUrlsList)
            {
                YouTubePage ytPage = new YouTubePage(video);
                ytPage.DownloadYouTubePage();
                YouTubeCommentsPage ytComPage = new YouTubeCommentsPage(video);
                ytComPage.DownloadYouTubeCommentsPage();
                YouTubeVideo ytv = new YouTubeVideo(video, ytPage, ytComPage);
                videosList.Add(ytv);
            }
            return new YouTubeVideoGroup(videosList);
        }

        private void GetTotalDuration()
        {
            Duration = TimeSpan.Zero;
            var tsMatches = Regex.Matches(_playlistPage,
                @"<div\sclass=""timestamp"">[^>]*>(?<TS>[^<]*)");
            foreach (Match match in tsMatches)
            {
                string matchDuration;
                if (match.Groups["TS"].Value.Length == 4)       // 0:ss -> 00:00:ss
                    matchDuration = "00:0" + match.Groups["TS"].Value;
                else if (match.Groups["TS"].Value.Length == 5)  // mm:ss -> 00:mm:ss
                    matchDuration = "00:" + match.Groups["TS"].Value;
                else
                    matchDuration = match.Groups["TS"].Value;   // String is already in hh:mm:ss format
                Duration += TimeSpan.Parse(matchDuration);
            }
        }
        private string GetPlaylistId(string playlistUrl)
        {
            var plIdMatch = Regex.Match(playlistUrl, @"list=(?<list>[^&]*)");
            return plIdMatch.Groups["list"].Value;
        } 
        private void GetPlaylistInformation()
        {
            GetTitle();
            var detailsMatch = Regex.Match(_playlistPage,
                @"<ul\sclass=""pl-header-details""><li>by\s<a\shref=[^>]*>(?<owner>[^<]*)</a></li><li>(?<numvideos>\S*)[^<]*<\/li><li>(?<views>\S*)[^<]*<\/li><li>(Last\supdated\son\s(?<updated>[^<]*)|Updated\s(?<updated>[^<]*))");
            Owner = WebUtility.HtmlDecode(detailsMatch.Groups["owner"].Value);
            int numVideosValue, viewsValue;

            int.TryParse(detailsMatch.Groups["numvideos"].Value, out numVideosValue);
            int.TryParse(detailsMatch.Groups["views"].Value, NumberStyles.AllowThousands, null, out viewsValue);
            NumVideos = numVideosValue;
            Views = viewsValue;
            DateTime dateUpdatedValue;
            if (!DateTime.TryParse(detailsMatch.Groups["updated"].Value, out dateUpdatedValue))
                dateUpdatedValue = DateTime.Today;
            LastUpdated = dateUpdatedValue;
        }
        private void GetTitle()
        {
            var titleMatch = Regex.Match(_playlistPage, @"<h1\sclass=""pl-header-title"">\s*(?<title>[^\r\n]*)");
            string title = titleMatch.Groups["title"].Value;
            Title = WebUtility.HtmlDecode(title);
        }

        private void GetAllYouTubeUrls()
        {
            var urlMatches = Regex.Matches(_playlistPage,
                @"<td\sclass=""pl-video-title"">\s*<a\s([\w-]*=""[^""]*""\s?)*");
            // TODO This hardcoding should be improved.
            foreach (Match match in urlMatches) 
            {
                string href = match.Groups[1].Captures[2].Value;
                string vidId = href.Substring(15, 11);
                _pageUrls.Add(new YouTubeURL(@"https://www.youtube.com/watch?v=" + vidId));
            }
        }

    }
}
