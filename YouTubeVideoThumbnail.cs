using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace YouTubeParse
{
    /// <summary>
    /// Properties and events related to grabbing and storing YouTube thumbnail images.
    /// </summary>
    public class YouTubeVideoThumbnail
    {
        /// <summary>
        /// Instance of a YouTube video URL for this instance of a YouTubeVideoThumbnail
        /// </summary>
        public YouTubeUrl VideoUrl { get; private set; }
        /// <summary>
        /// URI for full resolution thumbnail image
        /// </summary>
        public Uri ImageMaxResUri { get; private set; }
        /// <summary>
        /// URI for lower-quality thumbnail image
        /// </summary>
        public Uri ImageAlternateUri { get; private set; }
        private BitmapImage _thumbnailImage;
        /// <summary>
        /// Downloaded thumbnail for this YouTubeURL
        /// </summary>
        public BitmapImage ThumbnailImage { get { return _thumbnailImage; } }
        /// <summary>
        /// Gets whether this instance's thumbnail image is the maximum resolution version, or the lower resolution alternative
        /// </summary>
        public bool UsingMaxResImage { get; private set; }
        /// <summary>
        /// Initializes a new instance of the YouTubeVideoThumbnail class. Provides members related to storing a YouTubeURL, URLs to video thumbnail images, and events related to downloading them.
        /// </summary>
        /// <param name="youtubeuUrl">A validated YouTube URL. (Use YouTubeURL.ValidateYTURL)</param>
        public YouTubeVideoThumbnail(string youtubeuUrl)
        {
            VideoUrl = new YouTubeUrl(youtubeuUrl);
            CreateURLs();
            GetThumbnail();
        }
        /// <summary>
        /// Generates the URLs and URIs needed to access thumbnail images.
        /// </summary>
        private void CreateURLs()
        {
            ImageMaxResUri = new Uri(String.Format("http://i.ytimg.com/vi/{0:ID}/maxresdefault.jpg", VideoUrl), UriKind.Absolute);
            ImageAlternateUri = new Uri(String.Format("http://i.ytimg.com/vi/{0:ID}/0.jpg", VideoUrl), UriKind.Absolute);
        }
        /// <summary>
        /// Initiates attempts to download the thumbnail image for this VideoURL. If the first attempt fails, a second attempt is made with an alternate URL.
        /// </summary>
        private void GetThumbnail()
        {
            _thumbnailImage = new BitmapImage(ImageMaxResUri);
            _thumbnailImage.DownloadCompleted += OnThumbnailSuccess;
            _thumbnailImage.DownloadFailed += _thumbnailImage_DownloadFailed;
        }
        /// <summary>
        /// Attempts to download lower-quality thumbnail if the maxresdefault attempt fails.
        /// </summary>
        /// <param name="sender">Passthrough for the object from the first attempt.</param>
        /// <param name="e">Passthrough for the EventArgs from the first attempt.</param>
        void _thumbnailImage_DownloadFailed(object sender, ExceptionEventArgs e)
        {
            _thumbnailImage = new BitmapImage(ImageAlternateUri);
            _thumbnailImage.DownloadCompleted += OnThumbnailSuccess;
            _thumbnailImage.DownloadFailed += OnThumbnailFailure;
        }
        /// <summary>
        /// Occurs when the YouTube thumbnail image has been completely downloaded.
        /// </summary>
        public event EventHandler GetThumbnailSuccess;
        private void OnThumbnailSuccess(object sender, EventArgs e)
        {
            UsingMaxResImage = true;
            EventHandler getThumbnailSuccess = GetThumbnailSuccess;
            if (getThumbnailSuccess != null)
                GetThumbnailSuccess(sender, e);
        }
        /// <summary>
        /// Occurs when the YouTube thumbnail image has failed to download.
        /// </summary>
        public event EventHandler<ExceptionEventArgs> GetThumbailFailure;
        private void OnThumbnailFailure(object sender, ExceptionEventArgs exception)
        {
            EventHandler<ExceptionEventArgs> getThumbailFailure = GetThumbailFailure;
            if (getThumbailFailure != null)
                GetThumbailFailure(sender, exception);
        }
    }
}
