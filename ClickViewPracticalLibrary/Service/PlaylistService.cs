using ClickViewPracticalLibrary.Model;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ClickViewPracticalLibrary.Service
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IPlaylistStore _store;
        private readonly ILogger<PlaylistService> _log;

        public PlaylistService(IPlaylistStore store, ILogger<PlaylistService> logger)
        {
            _store = store;
            _log = logger;
        }


        //Adds a playlist if it has all valid video ids, id <=0 and a name
        public HttpStatusCode AddPlaylist(Playlist playlist)
        {
            if (playlist.ID > 0)
            {
                LogError("Playlist ID greater than 0", nameof(AddPlaylist));
                return HttpStatusCode.BadRequest;
            }

            if (string.IsNullOrEmpty(playlist.Name))
            {
                LogError("Playlist name is empty", nameof(AddPlaylist));
                return HttpStatusCode.BadRequest;
            }

            if (playlist.VideoIds.Any())
            {
                var videos = _store.GetVideos(new VideoPlaylistFilter { Ids = playlist.VideoIds }).Select(o => o.Id).ToList();
                if (videos.Count != playlist.VideoIds.Count)
                {
                    var invalidIds = playlist.VideoIds.Where(o => !videos.Contains(o));
                    LogError($"One of more invalid Video Ids were provided ({string.Join(',', invalidIds)})", nameof(AddPlaylist));
                    return HttpStatusCode.BadRequest;
                }
            }

            playlist.ID = GetAllPlaylists().Max(o => o.ID) + 1;
            _store.AddPlaylist(playlist);
            return HttpStatusCode.OK;
        }

        //Update an existing playlist. Must have an id greater than 0, a name and all valid video ids
        public HttpStatusCode UpdatePlaylist(Playlist playlist)
        {
            if (playlist.ID <= 0)
            {
                LogError("Playlist ID less than 0", nameof(UpdatePlaylist));
                return HttpStatusCode.BadRequest;
            }

            if (string.IsNullOrEmpty(playlist.Name))
            {
                LogError("Playlist name is empty", nameof(UpdatePlaylist));
                return HttpStatusCode.BadRequest;
            }

            var existingPlayList = _store.GetPlaylistIfExists(playlist.ID);
            if (existingPlayList == null)
            {
                LogError($"Playlist not found for {playlist.ID}", nameof(UpdatePlaylist));
                return HttpStatusCode.NotFound;
            }

            if (playlist.VideoIds.Any())
            {
                var videos = _store.GetVideos(new VideoPlaylistFilter { Ids = playlist.VideoIds }).Select(o => o.Id).ToList();
                if (videos.Count != playlist.VideoIds.Count)
                {
                    var invalidIds = playlist.VideoIds.Where(o => !videos.Contains(o));
                    LogError($"One of more invalid Video Ids were provided ({string.Join(',', invalidIds)})", nameof(UpdatePlaylist));
                    return HttpStatusCode.BadRequest;
                }
            }

            existingPlayList.Name = playlist.Name;
            existingPlayList.Description = playlist.Description;
            existingPlayList.VideoIds = playlist.VideoIds;
            return HttpStatusCode.OK;
        }

        //Will Delete a Playlist if id > 0 & Playlist exists
        public HttpStatusCode DeletePlaylist(int playlistId)
        {
            if (playlistId <= 0)
            {
                LogError("Playlist ID less than 0", nameof(DeletePlaylist));
                return HttpStatusCode.BadRequest;
            }

            var existingPlaylist = _store.GetPlaylistIfExists(playlistId);
            if(existingPlaylist == null)
            {
                LogError($"Playlist not found for id {playlistId}", nameof(DeletePlaylist));
                return HttpStatusCode.NotFound;
            }

            _store.RemovePlaylist(existingPlaylist);
            return HttpStatusCode.OK;
        }


        //Will add video to playlist if both ids > 0, both exist & playlist doesn't already contain video.
        public HttpStatusCode AddVideoToPlaylist(int videoId, int playlistId)
        {
            if(videoId <= 0 || playlistId <= 0)
            {
                LogError($"Playlist - {playlistId} or Video ID - {videoId} less than 0", nameof(AddVideoToPlaylist));
                return HttpStatusCode.BadRequest;
            }

            var videoToAdd = _store.GetVideoIfExists(videoId);
            if (videoToAdd == null)
            {
                LogError($"Video not found for {videoId}", nameof(AddVideoToPlaylist));
                return HttpStatusCode.NotFound;
            }

            var existingPlayList = _store.GetPlaylistIfExists(playlistId);
            if (existingPlayList == null)
            {
                LogError($"Playlist not found for {playlistId}", nameof(AddVideoToPlaylist));
                return HttpStatusCode.NotFound;
            }

            if (existingPlayList.VideoIds.Contains(videoId))
            {
                LogError($"Playlist {playlistId} already contains video ID {videoId}", nameof(AddVideoToPlaylist));
                return HttpStatusCode.BadRequest;
            }

            existingPlayList.VideoIds.Add(videoId);
            return HttpStatusCode.OK;
        }

        //Remove a video from a playlist if both ids > 0, playlist exists and playlist contains video
        public HttpStatusCode RemoveVideoFromPlaylist(int videoId, int playlistId)
        {
            if (videoId <= 0 || playlistId <= 0)
            {
                LogError($"Playlist - {playlistId} or Video ID - {videoId} less than 0", nameof(RemoveVideoFromPlaylist));
                return HttpStatusCode.BadRequest;
            }

            var existingPlayList = _store.GetPlaylistIfExists(playlistId);
            if (existingPlayList == null)
            {
                LogError($"Playlist not found for {playlistId}", nameof(RemoveVideoFromPlaylist));
                return HttpStatusCode.NotFound;
            }

            if (!existingPlayList.VideoIds.Contains(videoId))
            {
                LogError($"Playlist {playlistId} does not contain video ID {videoId}", nameof(RemoveVideoFromPlaylist));
                return HttpStatusCode.BadRequest;
            }

            existingPlayList.VideoIds.Remove(videoId);
            return HttpStatusCode.OK;
        }

        //Get all related videos from a playlist id
        public List<Video> GetAllVideosInPlaylist(int playlistId)
        {
            if (playlistId <= 0)
            {
                LogError("Playlist ID less than 0", nameof(RemoveVideoFromPlaylist));
                return new List<Video>();
            }

            var existingPlayList = _store.GetPlaylistIfExists(playlistId);
            if (existingPlayList == null)
            {
                LogError($"Playlist not found for {playlistId}", nameof(GetAllVideosInPlaylist));
                return new List<Video>();
            }

            return _store.GetVideos(new VideoPlaylistFilter { Ids = existingPlayList.VideoIds});
        }

        public List<Playlist> GetAllPlaylists()
        {
            return _store.GetPlaylists(new VideoPlaylistFilter());
        }

        public List<Video> GetAllVideos()
        {
            return _store.GetVideos(new VideoPlaylistFilter()).ToList();
        }

        private void LogError(string message, string method)
        {
            _log.LogError("{message} - within method {method}", message, method);
        }
    }
}
