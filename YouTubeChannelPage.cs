using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YouTubeParse
{
    public class YouTubeChannelPage : YouTubeHtmlPage
    {
        //private string _channelAboutPage;
        /// <summary>
        /// ID of this channel
        /// </summary>
        public string ChannelId { get; private set; }
        /// <summary>
        /// Name of the user channel
        /// </summary>
        public string Title { get; private set; }
        /// <summary>
        /// Uri to this user channel page
        /// </summary>
        public Uri Uri { get; private set; }
        /// <summary>
        /// Number of subscribers to this channel
        /// </summary>
        public int Subscribers { get; private set; }
        /// <summary>
        /// Number of views for all videos on this channel
        /// </summary>
        public int Views { get; private set; }
        /// <summary>
        /// Date the user created their YouTube account
        /// </summary>
        public DateTime JoinedDate { get; private set; }
        /// <summary>
        /// Instantiates a new object for retrieving information about a YouTube user channel
        /// </summary>
        /// <param name="channelUrl"></param>
        public YouTubeChannelPage(string channelUrl ) : base (channelUrl)
        {
            
            if (!ValidateChannelUrl(channelUrl))
                throw new ArgumentException("Invalid YouTube channel URL", nameof(channelUrl));
            Uri = new Uri(channelUrl, UriKind.Absolute);
            GetChannelId(channelUrl);

            throw new NotImplementedException("This class is not ready for prime-time");
        }
        /// <summary>
        /// Validates a string as a YouTube channel URL
        /// </summary>
        /// <param name="url">String to validate</param>
        /// <returns>Whether the string is a valid YouTube user channel</returns>
        public static bool ValidateChannelUrl(string url)
        {
            if (url.Contains(@"youtube.com") &&
                (url.Contains(@"/user/") || url.Contains(@"/channel/")))
                return true;
            return false;
        }
        private void GetChannelId(string channelUrl)
        {
            var plIdMatch = Regex.Match(channelUrl, @"(?:channel|user)/(?<id>[^/]*/)");
            ChannelId = plIdMatch.Groups["id"].Value;
        }

    }
}
