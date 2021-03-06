﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeParse
{
    public class YouTubeHtmlPage
    {
        protected string Page;
        public bool IsPageDownloaded { get; protected set; }
        public Uri PageUri { get; }
        /// <summary>
        /// Downloads a YouTubeVideoPage, YouTubeCommentsPage, YouTubePlaylistPage or YouTubeChannelPage
        /// </summary>
        /// <param name="url"></param>
        public YouTubeHtmlPage(string url)
        {
            if (YouTubePlaylistPage.ValidatePlaylistUrl(url) || YouTubeCommentsPage.ValidateYouTubeCommentsPageUrl(url)
                || YouTubeChannelPage.ValidateChannelUrl(url) || YouTubeUrl.ValidateUrl(url) 
                || YouTubeGetInfoPage.ValidateYouTubeGetInfoUrl(url))
            {
                PageUri = new Uri(url);
            }
            else
                throw new ArgumentException("URL could not be validated as a supported YouTube page type: " + url, nameof(url));
        }
        public virtual async Task DownloadPageAsync()
        {
            var downloader = new HttpDownloader(PageUri.AbsoluteUri, String.Empty, String.Empty);
            Page = await downloader.GetPageAsync();
            IsPageDownloaded = true;
        }
        public virtual async Task DownloadPageAsync(Uri uri)
        {
            var downloader = new HttpDownloader(uri.AbsoluteUri, String.Empty, String.Empty);
            Page = await downloader.GetPageAsync();
            IsPageDownloaded = true;
        }
    }
}
