using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeParse
{
    public class VideoGroupEventArgs
    {
        public VideoGroupEventArgs(YouTubeVideo video)
        {
            Video = video;
        }
        public YouTubeVideo Video { get; private set; } 
    }
    public class YouTubeVideoGroup
    {
        #region Public Properties

        public bool IsVideoGroupReady => _videosNotReady.Count == 0;

        /// <summary>
        /// List of all YouTubeVideos
        /// </summary>
        public IEnumerable<YouTubeVideo> VideoGroupList
        {
            get { return _videoGroupList; }
            private set { _videoGroupList = value; }
        }
        private IEnumerable<YouTubeVideo> _videoGroupList;

        public int Count => VideoGroupList.Count();

        /// <summary>
        /// List of video titles for all videos in this list
        /// </summary>
        public List<string> TitlesList
        {
            get { return VideoGroupList.Select(video => video.VideoPage.VideoTitle).ToList(); }
        }
        /// <summary>
        /// List of all long-form YouTube URLs for videos in this group.
        /// </summary>
        public List<Uri> UrisLongList
        {
            get { return VideoGroupList.Select(video => video.Url.LongYTURL).ToList(); }
        }
        /// <summary>
        /// List of all short-form YouTube URLs for videos in this group.
        /// </summary>
        public List<Uri> UrisShortList
        {
            get { return VideoGroupList.Select(video => video.Url.ShortYTURL).ToList(); }
        }
        /// <summary>
        /// Average number of views for videos in this group.
        /// </summary>
        public double ViewsAvg
        {
            get { return VideoGroupList.Average(video => video.VideoPage.ViewCount); }
        }
        //public List<YouTubeVideoThumbnail> ThumbnailsList { get; set; }
        /// <summary>
        /// Total number of views for all videos in this group.
        /// </summary>
        public int ViewsTotal
        {
            get { return VideoGroupList.Sum(video => video.VideoPage.ViewCount); }
        }

        /// <summary>
        /// Average number of comments for videos in this group.
        /// </summary>
        public double CommentsAvg
        {
            get { return VideoGroupList.Average(video => video.CommentsPage.NumComments); }
        }
        /// <summary>
        /// Total number of comments for all videos in this group.
        /// </summary>
        public int CommentsTotal
        {
            get { return VideoGroupList.Sum(video => video.CommentsPage.NumComments); }
        }
        /// <summary>
        /// Average duration of all videos in this group.
        /// </summary>
        public TimeSpan DurationAvg
        {
            get
            {
                return new TimeSpan(0, 0, 0,
                    (int)VideoGroupList.Average(video => video.VideoPage.Duration.TotalSeconds));
            }
        }
        /// <summary>
        /// Total duration of all videos in this group.
        /// </summary>
        public TimeSpan DurationTotal
        {
            get { return new TimeSpan(0, 0, 0, 
                (int) VideoGroupList.Sum(video => video.VideoPage.Duration.TotalSeconds)); }
        }
        #endregion

        #region Private Fields
        private List<YouTubeVideo> _videosNotReady;
        #endregion

        public YouTubeVideoGroup(IEnumerable<YouTubeVideo> videosList)
        {
            VideoGroupList = videosList;
            CheckForVideosNotReady();
        }
        
        #region Public Methods
        public async Task DownloadPagesForVideosNotReady()
        {
            foreach (var video in _videosNotReady)
            {
                try
                {
                    await video.DownloadPages();
                    OnVideoReady(new VideoGroupEventArgs(video));
                }
                catch (Exception e)
                {
                    throw;
                }
            }
            CheckForVideosNotReady();
        }
        /// <summary>
        /// Adds a video to the group
        /// </summary>
        /// <param name="video">A new YouTubeVideo to add to the group</param>
        /// <returns>Whether the video was successfully added. Returns false if the video is already in the group.</returns>
        public bool AddVideo(YouTubeVideo video)
        {
            var vgl = VideoGroupList.ToList();
            if (!vgl.Contains(video)) return false;
            vgl.Add(video);
            VideoGroupList = vgl;
            CheckForVideosNotReady();
            return true;
        }
        /// <summary>
        /// Adds multiple videos to the group.
        /// </summary>
        /// <param name="videos">An IEnumerable of YouTubeVideos to add to the group</param>
        /// <returns>Whether the videos were successfully added. Returns false if all videos are already in the group</returns>
        public bool AddVideos(IEnumerable<YouTubeVideo> videos)
        {
            var vgl = VideoGroupList.ToList();
            bool videoAdded = false;
            foreach (var video in videos)
            {
                if (vgl.Contains(video)) continue;
                vgl.Add(video);
                videoAdded = true;
            }
            VideoGroupList = vgl;
            CheckForVideosNotReady();
            return videoAdded;
        }
        /// <summary>
        /// Removes a video from the group
        /// </summary>
        /// <param name="video">A YouTubeVideo to remove from the group</param>
        /// <returns>Whether the group was successfully removed from the group</returns>
        public bool RemoveVideo(YouTubeVideo video)
        {
            var vgl = VideoGroupList.ToList();
            if (!vgl.Contains(video)) return false;
            vgl.Remove(video);
            VideoGroupList = vgl;
            return true;
        }
        /// <summary>
        /// Removes several videos from the group
        /// </summary>
        /// <param name="videos">An IEnumerable of YouTubeVideos to remove from the group</param>
        /// <returns>Whether the videos were successfully removed.</returns>
        public bool RemoveVideos(IEnumerable<YouTubeVideo> videos)
        {
            var vgl = VideoGroupList.ToList();
            bool videosRemoved = false;
            foreach (var video in videos)
            {
                if (!vgl.Contains(video)) continue;
                vgl.Remove(video);
                videosRemoved = true;
            }
            VideoGroupList = vgl;
            return videosRemoved;
        }
        #endregion

        #region Private Methods
        private void CheckForVideosNotReady()
        {
            var videosNeedingDownload = from video in VideoGroupList
                                        where video.IsVideoReady
                                        select video;
            _videosNotReady = videosNeedingDownload.ToList();
        }

        #endregion
        public event EventHandler<VideoGroupEventArgs> VideoReady;

        protected virtual void OnVideoReady(VideoGroupEventArgs e)
        {
            VideoReady?.Invoke(this, e);
        }
    }
}
