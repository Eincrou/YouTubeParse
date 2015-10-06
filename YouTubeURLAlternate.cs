using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace YouTubeParse
{
    /// <summary>
    /// Parses and stores YouTube URLs and VideoIDs.
    /// </summary>
    public class YouTubeURLAlternate : IFormattable
    {
        private static readonly string[] _idPatterns =
            { 
                @"(?:\w*://.*)?youtube.com/watch\?(?:feature=player_embedded&)?v=([^\&\?\/]{11})", 
                @"(?:\w*://.*)?youtube.com/embed/([^\&\?\/]{11})",
                @"(?:\w*://.*)?youtube.com/v/([^\&\?\/]{11})",
                @"(?:\w*://.*)?youtu.be/([^\&\?\/]{11})",
                @"(?:\w*://.*)?youtube.com/verify_age\?next_url=watch%3Fv%3D([^\&\?\/]{11})",
                @"(?:\w*://.*)?youtube.com/watch\?annotation_id=annotation_\d*\&feature=iv&src_vid=[^\&\?\/]{11}\&v=([^\&\?\/]{11})",
                @"(?:\w*://.*)?interleave-vr.com/youtube-proper-player.php\?v=([^\&\?\/]{11})"
            };
        private string _inputURL;
        private string _videoID;
        /// <summary>
        /// This instance's unique video indentifier.
        /// </summary>
        public string VideoID { get { return _videoID; } }
        /// <summary>
        /// A full-length URL for this instance's VideoID.
        /// </summary>
        public string LongYTURL { get { return @"https://www.youtube.com/watch?v=" + _videoID; } }
        /// <summary>
        /// An abbreviated URL for this instance's VideoID.
        /// </summary>
        public string ShortYTURL { get { return @"http://youtu.be/" + _videoID; } }
        /// <summary>
        /// A URL to force the video to play with the Flash Video Player.
        /// </summary>
        public string EnforcerURL { get { return @"http://www.interleave-vr.com/youtube-proper-player.php?v=" + _videoID; } }
        /// <summary>
        /// Initializes an instance of the YouTubeURL class.
        /// </summary>
        /// <param name="inputURL">A valid URL for a YouTube video.</param>
        public YouTubeURLAlternate(string inputURL)
        {
            _inputURL = inputURL;
            _videoID = GetVideoID(inputURL);
        }
        /// <summary>
        /// Gets the unique identifier of a YouTube video from a valid YouTube video URL.
        /// </summary>
        /// <param name="url">A validated URL for a YouTube video.</param>
        /// <returns>Eleven-character unique identifier for the video.</returns>
        public static string GetVideoID(string url)
        {
            if(url.Contains("youtube.com") || url.Contains("youtu.be"))
            {
                string domain = url.Contains("youtube.com/") ? "youtube.com/" : "youtu.be/";
                string urlParams = url.Remove(0, url.IndexOf(domain) + domain.Length);
                if (urlParams.Length < 11)
                    throw new ArgumentException("Invalid YouTube video URL", "url");
                if (urlParams.Contains("?"))
                    urlParams = urlParams.Remove(urlParams.IndexOf(@"watch?"), 6);
                if (urlParams.Contains("="))
                {
                    var ytParameters = new Dictionary<string, string>();
                    string[] tempParams = urlParams.Split('&');
                    if (tempParams.Length > 0)
                        foreach (string pair in tempParams)
                        {
                            string[] temp = pair.Split('=');
                            ytParameters.Add(temp[0], temp[1]);
                        }
                    else
                    {
                        string[] temp = urlParams.Split('=');
                        ytParameters.Add(temp[0], temp[1]);
                    }
                    if (ytParameters.ContainsKey("v"))
                        return ytParameters["v"];
                    else
                        throw new ArgumentException("Invalid YouTube video URL", "url");
                }
                else if (urlParams.Contains("embed/") || urlParams.Contains("v/"))
                {
                    string temp = urlParams.Contains("embed/") ? "embed/" : "v/";
                    return urlParams.Remove(0, urlParams.IndexOf(temp) + temp.Length);
                }
                else
                    return urlParams;
            }
            else
                throw new ArgumentException("Invalid YouTube video URL", "url");
        }
        /// <summary>
        /// Checks if the input string can be successfully parsed into a YouTube VideoID.
        /// </summary>
        /// <param name="URLToCheck">A URL to check.</param>
        /// <returns>Whether the URL matches a supported YouTube URL pattern.</returns>
        public static bool ValidateYTURL(string URLToCheck)
        {
            try
            {
                GetVideoID(URLToCheck);                
            }
            catch (ArgumentException)
            {
                return false;
            }
            return true;                
        }
        public override string ToString()
        {
            return ShortYTURL;
        }
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null) format = "S";
            switch (format.ToUpper())
            {
                case null:
                case "ID":
                    return VideoID;
                case "S":
                    return ShortYTURL;
                case "L":
                    return LongYTURL;
                case "E":
                    return EnforcerURL;
                default:
                    throw new FormatException(String.Format(formatProvider,
                          "Format {0} is not supported", format));
            }
        }
    }
}
