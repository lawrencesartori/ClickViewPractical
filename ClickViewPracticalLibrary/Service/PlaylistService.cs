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
        public async Task<HttpStatusCode> AddPlaylistAsync(Playlist playlist)
        {
            if (playlist.ID > 0)
            {
                LogError("Playlist ID greater than 0", nameof(AddPlaylistAsync));
                return HttpStatusCode.BadRequest;
            }

            if (string.IsNullOrEmpty(playlist.Name))
            {
                LogError("Playlist name is empty", nameof(AddPlaylistAsync));
                return HttpStatusCode.BadRequest;
            }

            if (playlist.VideoIds.Any())
            {
                var videos = _store.GetVideos(new VideoPlaylistFilter { Ids = playlist.VideoIds }).Select(o => o.Id).ToList();
                if (videos.Count != playlist.VideoIds.Count)
                {
                    var invalidIds = playlist.VideoIds.Where(o => !videos.Contains(o));
                    LogError($"One of more invalid Video Ids were provided ({string.Join(',', invalidIds)})", nameof(AddPlaylistAsync));
                    return HttpStatusCode.BadRequest;
                }
            }

            playlist.ID = GetAllPlaylists().Max(o => o.ID) + 1;
            await _store.AddPlaylist(playlist);
            return HttpStatusCode.OK;
        }

        //Update an existing playlist. Must have an id greater than 0, a name and all valid video ids
        public async Task<HttpStatusCode> UpdatePlaylistAsync(Playlist playlist)
        {
            if (playlist.ID <= 0)
            {
                LogError("Playlist ID less than 0", nameof(UpdatePlaylistAsync));
                return HttpStatusCode.BadRequest;
            }

            if (string.IsNullOrEmpty(playlist.Name))
            {
                LogError("Playlist name is empty", nameof(UpdatePlaylistAsync));
                return HttpStatusCode.BadRequest;
            }

            var existingPlayList = _store.GetPlaylistIfExists(playlist.ID);
            if (existingPlayList == null)
            {
                LogError($"Playlist not found for {playlist.ID}", nameof(UpdatePlaylistAsync));
                return HttpStatusCode.NotFound;
            }

            if (playlist.VideoIds.Any())
            {
                var videos = _store.GetVideos(new VideoPlaylistFilter { Ids = playlist.VideoIds }).Select(o => o.Id).ToList();
                if (videos.Count != playlist.VideoIds.Count)
                {
                    var invalidIds = playlist.VideoIds.Where(o => !videos.Contains(o));
                    LogError($"One of more invalid Video Ids were provided ({string.Join(',', invalidIds)})", nameof(UpdatePlaylistAsync));
                    return HttpStatusCode.BadRequest;
                }
            }

            existingPlayList.Name = playlist.Name;
            existingPlayList.Description = playlist.Description;
            existingPlayList.VideoIds = playlist.VideoIds;
            await _store.SaveChangesAsync();
            return HttpStatusCode.OK;
        }

        //Will Delete a Playlist if id > 0 & Playlist exists
        public async Task<HttpStatusCode> DeletePlaylistAsync(int playlistId)
        {
            if (playlistId <= 0)
            {
                LogError("Playlist ID less than 0", nameof(DeletePlaylistAsync));
                return HttpStatusCode.BadRequest;
            }

            var existingPlaylist = _store.GetPlaylistIfExists(playlistId);
            if(existingPlaylist == null)
            {
                LogError($"Playlist not found for id {playlistId}", nameof(DeletePlaylistAsync));
                return HttpStatusCode.NotFound;
            }

            await _store.RemovePlaylist(existingPlaylist);
            return HttpStatusCode.OK;
        }


        //Will add video to playlist if both ids > 0, both exist & playlist doesn't already contain video.
        public async Task<HttpStatusCode> AddVideoToPlaylistAsync(int videoId, int playlistId)
        {
            if(videoId <= 0 || playlistId <= 0)
            {
                LogError($"Playlist - {playlistId} or Video ID - {videoId} less than 0", nameof(AddVideoToPlaylistAsync));
                return HttpStatusCode.BadRequest;
            }

            var videoToAdd = _store.GetVideoIfExists(videoId);
            if (videoToAdd == null)
            {
                LogError($"Video not found for {videoId}", nameof(AddVideoToPlaylistAsync));
                return HttpStatusCode.NotFound;
            }

            var existingPlayList = _store.GetPlaylistIfExists(playlistId);
            if (existingPlayList == null)
            {
                LogError($"Playlist not found for {playlistId}", nameof(AddVideoToPlaylistAsync));
                return HttpStatusCode.NotFound;
            }

            if (existingPlayList.VideoIds.Contains(videoId))
            {
                LogError($"Playlist {playlistId} already contains video ID {videoId}", nameof(AddVideoToPlaylistAsync));
                return HttpStatusCode.BadRequest;
            }

            existingPlayList.VideoIds.Add(videoId);
            await _store.SaveChangesAsync();
            return HttpStatusCode.OK;
        }

        //Remove a video from a playlist if both ids > 0, playlist exists and playlist contains video
        public async Task<HttpStatusCode> RemoveVideoFromPlaylistAsync(int videoId, int playlistId)
        {
            if (videoId <= 0 || playlistId <= 0)
            {
                LogError($"Playlist - {playlistId} or Video ID - {videoId} less than 0", nameof(RemoveVideoFromPlaylistAsync));
                return HttpStatusCode.BadRequest;
            }

            var existingPlayList = _store.GetPlaylistIfExists(playlistId);
            if (existingPlayList == null)
            {
                LogError($"Playlist not found for {playlistId}", nameof(RemoveVideoFromPlaylistAsync));
                return HttpStatusCode.NotFound;
            }

            if (!existingPlayList.VideoIds.Contains(videoId))
            {
                LogError($"Playlist {playlistId} does not contain video ID {videoId}", nameof(RemoveVideoFromPlaylistAsync));
                return HttpStatusCode.BadRequest;
            }

            existingPlayList.VideoIds.Remove(videoId);
            await _store.SaveChangesAsync();
            return HttpStatusCode.OK;
        }

        //Get all related videos from a playlist id
        public List<Video> GetAllVideosInPlaylist(int playlistId)
        {
            if (playlistId <= 0)
            {
                LogError("Playlist ID less than 0", nameof(RemoveVideoFromPlaylistAsync));
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
