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
    public class YouTubePlaylistPage : YouTubeHtmlPage
    {
        private readonly string _playlistId;

        /// <summary>
        /// URL to this playlist's main page
        /// </summary>
        public Uri PlaylistUrl
        {
            get { return new Uri(@"https://www.youtube.com/playlist?list=" + _playlistId); } 
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
        public List<YouTubeUrl> VideoUrlsList { get; } = new List<YouTubeUrl>();

        /// <summary>
        /// Instantiates a new YouTubePlaylist object, containing information about a playlist on YouTube.
        /// </summary>
        /// <param name="playlistUrl">String representing a valid YouTube playlist URL</param>
        public YouTubePlaylistPage(string playlistUrl) : base (playlistUrl)
        {
            if (!ValidatePlaylistUrl(playlistUrl))
                throw new ArgumentException("Invalid YouTube playlist URL", "playlistUrl");
            _playlistId = GetPlaylistId(playlistUrl);
        }
        /// <summary>
        /// Downloads and parses information about this playlist.  Throws exception if playlist is private.
        /// </summary>
        /// <returns></returns>
        public override async Task DownloadPageAsync()
        {
            await base.DownloadPageAsync(PlaylistUrl);
            if (await CheckIfPlaylistPrivate())
                throw new AccessViolationException("This playlist is private.");
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
        private string GetPlaylistId(string playlistUrl)
        {
            var plIdMatch = Regex.Match(playlistUrl, @"list=(?<list>[^&]*)");
            return plIdMatch.Groups["list"].Value;
        }
        private async void GetTotalDuration()
        {
            if (!IsPageDownloaded) await DownloadPageAsync();
            Duration = TimeSpan.Zero;
            var tsMatches = Regex.Matches(Page,
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
        //TODO Correctly parse: Last updated "3 days ago" / "Yesterday"
        private async void GetPlaylistInformation()
        {
            if (!IsPageDownloaded) await DownloadPageAsync();
            GetTitle();
            var detailsMatch = Regex.Match(Page,
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
        private async void GetTitle()
        {
            if (!IsPageDownloaded) await DownloadPageAsync();
            var titleMatch = Regex.Match(Page, @"<h1\sclass=""pl-header-title""[^>]*>\s*(?<title>[^\r\n]*)");
            string title = titleMatch.Groups["title"].Value;
            Title = WebUtility.HtmlDecode(title);
        }
        private async void GetAllYouTubeUrls()
        {
            if (!IsPageDownloaded) await DownloadPageAsync();
            var urlMatches = Regex.Matches(Page,
                @"<td\sclass=""pl-video-title"">\s*<a\s([\w-]*=""[^""]*""\s?)*");
            // TODO This hardcoding should be improved.
            foreach (Match match in urlMatches) 
            {
                string href = match.Groups[1].Captures[2].Value;
                string vidId = href.Substring(15, 11);
                VideoUrlsList.Add(new YouTubeUrl(@"https://www.youtube.com/watch?v=" + vidId));
            }
        }

        private async Task<bool> CheckIfPlaylistPrivate()
        {
            if (!IsPageDownloaded) await DownloadPageAsync();
            var privateMatch = Regex.Match(Page, @"This playlist is private");
            return privateMatch.Success;
        }
    }
}
