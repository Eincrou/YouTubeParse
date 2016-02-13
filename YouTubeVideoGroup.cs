using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeParse
{
    public class VideoGroupEventArgs
    {
        public VideoGroupEventArgs(YouTubeVideo video, int progress)
        {
            Video = video;
            Progress = progress;
        }
        public YouTubeVideo Video { get; }
        public int Progress { get; }
    }
    public class YouTubeVideoGroup
    {
        #region Public Properties

        public bool IsVideoGroupReady => _videosNotReady.Count == 0;

        /// <summary>
        /// List of all YouTubeVideos
        /// </summary>
        public List<YouTubeVideo> VideoGroupList
        {
            get { return _videoGroupList; }
            private set { _videoGroupList = value; }
        }
        private List<YouTubeVideo> _videoGroupList;

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
        /// <summary>
        /// Instantiates a new group of YouTubeVideos. Provides aggregated information about the set of videos.
        /// </summary>
        /// <param name="videos">A collection of YouTubeVideos</param>
        public YouTubeVideoGroup(IEnumerable<YouTubeVideo> videos)
        {
            VideoGroupList = videos.ToList();
            CheckVideosReady();
        }
        /// <summary>
        /// Instantiates a new group of YouTubeVideos. Provides aggregated information about the set of videos.
        /// </summary>
        /// <param name="videoUrls">A collection of YouTubeUrls</param>
        public YouTubeVideoGroup(IEnumerable<YouTubeUrl> videoUrls)
        {
            VideoGroupList = videoUrls.Select(url => new YouTubeVideo(url)).ToList();
            CheckVideosReady();
        }
        #region Public Methods
        public async Task DownloadPagesForVideosNotReadyAsync()
        {
            if (_videosNotReady?.Count > 0)
            {
                double progress = 0;
                foreach (var video in _videosNotReady)
                {
                    try
                    {
                        progress++;
                        await video.DownloadPages();
                        VideoGroupList.Add(video);
                        int progressAmount = Convert.ToInt32(((progress / _videosNotReady.Count) * 100));
                        OnVideoReady(new VideoGroupEventArgs(video, progressAmount));
                    }
                    catch(AccessViolationException ave)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                }
                CheckVideosReady();
            }
        }
        /// <summary>
        /// Adds a video to the group
        /// </summary>
        /// <param name="video">A new YouTubeVideo to add to the group</param>
        /// <returns>Whether the video was successfully added. Returns false if the video is already in the group.</returns>
        public bool AddVideo(YouTubeVideo video)
        {
            if (!VideoGroupList.Contains(video)) return false;
            VideoGroupList.Add(video);
            CheckVideosReady();
            return true;
        }
        /// <summary>
        /// Adds multiple videos to the group.
        /// </summary>
        /// <param name="videos">An IEnumerable of YouTubeVideos to add to the group</param>
        /// <returns>Whether the videos were successfully added. Returns false if all videos are already in the group</returns>
        public bool AddVideos(IEnumerable<YouTubeVideo> videos)
        {
            bool videoAdded = false;
            foreach (var video in videos)
            {
                if (VideoGroupList.Contains(video)) continue;
                VideoGroupList.Add(video);
                videoAdded = true;
            }
            CheckVideosReady();
            return videoAdded;
        }
        /// <summary>
        /// Removes a video from the group
        /// </summary>
        /// <param name="video">A YouTubeVideo to remove from the group</param>
        /// <returns>Whether the group was successfully removed from the group</returns>
        public bool RemoveVideo(YouTubeVideo video)
        {
            if (!VideoGroupList.Contains(video)) return false;
            VideoGroupList.Remove(video);
            return true;
        }
        /// <summary>
        /// Removes several videos from the group
        /// </summary>
        /// <param name="videos">An IEnumerable of YouTubeVideos to remove from the group</param>
        /// <returns>Whether the videos were successfully removed.</returns>
        public bool RemoveVideos(IEnumerable<YouTubeVideo> videos)
        {
            bool videosRemoved = false;
            foreach (var video in videos)
            {
                if (!VideoGroupList.Contains(video)) continue;
                VideoGroupList.Remove(video);
                videosRemoved = true;
            }
            return videosRemoved;
        }
        #endregion

        #region Private Methods
        private void CheckVideosReady()
        {
            var videosNeedingDownload = from video in VideoGroupList
                                        where !video.IsVideoReady
                                        select video;
            _videosNotReady = videosNeedingDownload.ToList();
            VideoGroupList.RemoveAll(video => _videosNotReady.Contains(video));
        }

        #endregion
        public event EventHandler<VideoGroupEventArgs> VideoReady;

        protected virtual void OnVideoReady(VideoGroupEventArgs e)
        {
            VideoReady?.Invoke(this, e);
        }
    }
}
