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
                _log.LogError("Playlist ID greater than 0", nameof(AddPlaylistAsync));
                return HttpStatusCode.BadRequest;
            }

            if (string.IsNullOrEmpty(playlist.Name))
            {
                _log.LogError("Playlist name is empty", nameof(AddPlaylistAsync));
                return HttpStatusCode.BadRequest;
            }

            if (playlist.VideoIds.Any())
            {
                var videos = _store.GetVideos(new VideoPlaylistFilter { Ids = playlist.VideoIds }).Select(o => o.Id).ToList();
                if (videos.Count != playlist.VideoIds.Count)
                {
                    var invalidIds = playlist.VideoIds.Where(o => !videos.Contains(o));
                    _log.LogError($"One of more invalid Video Ids were provided ({string.Join(',', invalidIds)})", nameof(AddPlaylistAsync));
                    return HttpStatusCode.BadRequest;
                }
            }

            return await _store.AddPlaylistAsync(playlist);
        }

        //Update an existing playlist. Must have an id greater than 0, a name and all valid video ids
        public async Task<HttpStatusCode> UpdatePlaylistAsync(Playlist playlist)
        {
            if (playlist.ID <= 0)
            {
                _log.LogError("Playlist ID less than 0", nameof(UpdatePlaylistAsync));
                return HttpStatusCode.BadRequest;
            }

            if (string.IsNullOrEmpty(playlist.Name))
            {
                _log.LogError("Playlist name is empty", nameof(UpdatePlaylistAsync));
                return HttpStatusCode.BadRequest;
            }

            var existingPlayList = _store.GetPlaylistIfExists(playlist.ID);
            if (existingPlayList == null)
            {
                _log.LogError($"Playlist not found for {playlist.ID}", nameof(UpdatePlaylistAsync));
                return HttpStatusCode.NotFound;
            }

            if (playlist.VideoIds.Any())
            {
                var videos = _store.GetVideos(new VideoPlaylistFilter { Ids = playlist.VideoIds }).Select(o => o.Id).ToList();
                if (videos.Count != playlist.VideoIds.Count)
                {
                    var invalidIds = playlist.VideoIds.Where(o => !videos.Contains(o));
                    _log.LogError($"One of more invalid Video Ids were provided ({string.Join(',', invalidIds)})", nameof(UpdatePlaylistAsync));
                    return HttpStatusCode.BadRequest;
                }
            }

            return await _store.UpdatePlaylistAsync(playlist);
        }

        //Will Delete a Playlist if id > 0 & Playlist exists
        public async Task<HttpStatusCode> DeletePlaylistAsync(VideoPlaylistApiModel model)
        {
            if (model == null || model.playlistId <= 0)
            {
                _log.LogError("Playlist ID less than 0", nameof(DeletePlaylistAsync));
                return HttpStatusCode.BadRequest;
            }

            var existingPlaylist = _store.GetPlaylistIfExists(model.playlistId);
            if(existingPlaylist == null)
            {
                _log.LogError($"Playlist not found for id {model.playlistId}", nameof(DeletePlaylistAsync));
                return HttpStatusCode.NotFound;
            }

            return await _store.RemovePlaylistAsync(model.playlistId);
        }


        //Will add video to playlist if both ids > 0, both exist & playlist doesn't already contain video.
        public async Task<HttpStatusCode> AddVideoToPlaylistAsync(VideoPlaylistApiModel model)
        {
            if(model == null || model.videoId.IsNullOrZero() || model.playlistId <= 0)
            {
                _log.LogError($"Playlist - {model.playlistId} or Video ID - {model.videoId} less than 0", nameof(AddVideoToPlaylistAsync));
                return HttpStatusCode.BadRequest;
            }

            var videoToAdd = _store.GetVideoIfExists(model.videoId.Value);
            if (videoToAdd == null)
            {
                _log.LogError($"Video not found for {model.videoId}", nameof(AddVideoToPlaylistAsync));
                return HttpStatusCode.NotFound;
            }

            var existingPlayList = _store.GetPlaylistIfExists(model.playlistId);
            if (existingPlayList == null)
            {
                _log.LogError($"Playlist not found for {model.playlistId}", nameof(AddVideoToPlaylistAsync));
                return HttpStatusCode.NotFound;
            }

            if (existingPlayList.VideoIds.Contains(model.videoId.Value))
            {
                _log.LogError($"Playlist {model.playlistId} already contains video ID {model.videoId}", nameof(AddVideoToPlaylistAsync));
                return HttpStatusCode.BadRequest;
            }

            await _store.AddVideoToPlaylistAsync(model.videoId.Value, model.playlistId);
            return HttpStatusCode.OK;
        }

        //Remove a video from a playlist if both ids > 0, playlist exists and playlist contains video
        public async Task<HttpStatusCode> RemoveVideoFromPlaylistAsync(VideoPlaylistApiModel model)
        {
            if (model == null || model.videoId.IsNullOrZero() || model.playlistId <= 0)
            {
                _log.LogError($"Playlist - {model?.playlistId} or Video ID - {model?.videoId} less than 0", nameof(RemoveVideoFromPlaylistAsync));
                return HttpStatusCode.BadRequest;
            }

            var existingPlayList = _store.GetPlaylistIfExists(model.playlistId);
            if (existingPlayList == null)
            {
                _log.LogError($"Playlist not found for {model.playlistId}", nameof(RemoveVideoFromPlaylistAsync));
                return HttpStatusCode.NotFound;
            }

            if (!existingPlayList.VideoIds.Contains(model.videoId.Value))
            {
                _log.LogError($"Playlist {model.playlistId} does not contain video ID {model.videoId}", nameof(RemoveVideoFromPlaylistAsync));
                return HttpStatusCode.BadRequest;
            }

           return await _store.RemoveVideoFromPlaylistAsync(model.videoId.Value, model.playlistId);
        }

        //Get all related videos from a playlist id
        public List<Video> GetAllVideosInPlaylist(int playlistId)
        {
            if (playlistId <= 0)
            {
                _log.LogError("Playlist ID less than 0", nameof(RemoveVideoFromPlaylistAsync));
                return new List<Video>();
            }

            var existingPlayList = _store.GetPlaylistIfExists(playlistId);
            if (existingPlayList == null)
            {
                _log.LogError($"Playlist not found for {playlistId}", nameof(GetAllVideosInPlaylist));
                return new List<Video>();
            }

            return _store.GetVideos(new VideoPlaylistFilter { Ids = existingPlayList.VideoIds});
        }

        public List<Playlist> GetAllPlaylistsForVideo(int videoId)
        {
            if(videoId <= 0)
            {
                _log.LogError("Video ID less than 0", nameof(GetAllPlaylistsForVideo));
                return new List<Playlist>();
            }

            return _store.GetPlaylists(new PlaylistFilter { VideoId = videoId });
        }

        public List<Playlist> GetAllPlaylists()
        {
            return _store.GetPlaylists(new PlaylistFilter());
        }

        public List<Video> GetAllVideos()
        {
            return _store.GetVideos(new VideoPlaylistFilter()).ToList();
        }
    }
}
