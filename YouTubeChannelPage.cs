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

        public string ChannelId { get; private set; }
        public string Title { get; private set; }
        public Uri Uri { get; private set; }
        public int Subscribers { get; private set; }
        public int Views { get; private set; }
        public DateTime JoinedDate { get; private set; }

        public YouTubeChannelPage(string channelUrl ) : base (channelUrl)
        {
            
            if (!ValidateChannelUrl(channelUrl))
                throw new ArgumentException("Invalid YouTube channel URL", "channelUrl");
            Uri = new Uri(channelUrl, UriKind.Absolute);
            GetChannelId(channelUrl);

            throw new NotImplementedException("This class is not ready for prime-time");
        }

        private void GetChannelId(string channelUrl)
        {
            var plIdMatch = Regex.Match(channelUrl, @"(?:channel|user)/(?<id>[^/]*/)");
            ChannelId = plIdMatch.Groups["id"].Value;
        }
        public static bool ValidateChannelUrl(string url)
        {
            if (url.Contains(@"youtube.com") && 
                (url.Contains(@"/user/") || url.Contains(@"/channel/")))
                return true;
            return false;
        }
    }
}
