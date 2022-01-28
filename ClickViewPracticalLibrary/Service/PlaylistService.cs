using ClickViewPracticalLibrary.Model;
using Microsoft.Extensions.Logging;
using ClickViewPracticalLibrary.Model.Filters;

namespace ClickViewPracticalLibrary.Service
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IPlaylistStore _store;
        private readonly ILogger<PlaylistService> _log;
        private readonly MethodResultHelper _resultHelper;

        public PlaylistService(IPlaylistStore store, ILogger<PlaylistService> logger)
        {
            _store = store;
            _log = logger;
            _resultHelper = new MethodResultHelper(_log);
        }


        //Adds a playlist if it has all valid video ids, id <=0 and a name
        public async Task<MethodResult> AddPlaylistAsync(SimplePlaylist playlist)
        {
	        if (playlist.Id > 0)
	        {
		        return _resultHelper.GetBadRequestFailedResult("Playlist ID cannot be greater than 0", nameof(AddPlaylistAsync));
            }

            if (string.IsNullOrEmpty(playlist.Name))
            {
	            return _resultHelper.GetBadRequestFailedResult("Playlist name is empty", nameof(AddPlaylistAsync));
            }

            var newPlaylist = await _store.AddPlaylistAsync(new Playlist { VideoIds = new List<int>(), Description = playlist.Description, Name = playlist.Name});
            return newPlaylist != null ? _resultHelper.GetPlaylistSuccessResult(newPlaylist) : _resultHelper.GetNotFoundFailedResult("There was an issue creating your playlist", nameof(AddPlaylistAsync));
        }

        //Update an existing playlist. Must have an id greater than 0, a name and all valid video ids
        public async Task<MethodResult> UpdatePlaylistAsync(SimplePlaylist playlist)
        {
            if (playlist.Id <= 0)
            {
	            return _resultHelper.GetBadRequestFailedResult("Playlist ID cannot be greater than 0", nameof(UpdatePlaylistAsync));
            }

            if (string.IsNullOrEmpty(playlist.Name))
            {
	            return _resultHelper.GetBadRequestFailedResult("Playlist name is empty", nameof(UpdatePlaylistAsync));
            }

            var existingPlayList = await _store.GetPlaylistIfExists(playlist.Id);
            if (existingPlayList == null)
            {
	            return _resultHelper.GetNotFoundFailedResult($"Playlist not found for {playlist.Id}", nameof(UpdatePlaylistAsync));
            }
          

            var playlistResult = await _store.UpdatePlaylistAsync(playlist);
            return playlistResult != null
	            ? _resultHelper.GetPlaylistSuccessResult(playlistResult)
	            : _resultHelper.GetNotFoundFailedResult("Failed to update Playlist", nameof(UpdatePlaylistAsync));
        }

        //Will Delete a Playlist if id > 0 & Playlist exists
        public async Task<MethodResult> DeletePlaylistAsync(int playlistId)
        {
            if (playlistId <= 0)
            {
                return _resultHelper.GetBadRequestFailedResult("Playlist ID less than 0", nameof(DeletePlaylistAsync));
            }

            var existingPlaylist = await _store.GetPlaylistIfExists(playlistId);
            if(existingPlaylist == null)
            {
	            return _resultHelper.GetNotFoundFailedResult($"Playlist not found for id {playlistId}", nameof(DeletePlaylistAsync));
            }

            var result = await _store.RemovePlaylistAsync(playlistId);
            return result
	            ? _resultHelper.GetEmptySuccessResult()
	            : _resultHelper.GetNotFoundFailedResult("Failed to Delete Playlist", nameof(DeletePlaylistAsync));
        }


        //Will add video to playlist if both ids > 0, both exist & playlist doesn't already contain video.
        public async Task<MethodResult> AddVideoToPlaylistAsync(int playlistId, int videoId)
        {
            if(videoId <= 0 || playlistId <= 0)
            {
	            return _resultHelper.GetBadRequestFailedResult($"Playlist - {playlistId} or Video ID - {videoId} less than 0", nameof(AddVideoToPlaylistAsync));
            }

            var videoToAdd = await _store.GetVideoIfExists(videoId);
            if (videoToAdd == null)
            {
	            return _resultHelper.GetNotFoundFailedResult($"Video not found for {videoId}", nameof(AddVideoToPlaylistAsync));
            }

            var existingPlayList = await _store.GetPlaylistIfExists(playlistId);
            if (existingPlayList == null)
            {
	            return _resultHelper.GetNotFoundFailedResult($"Playlist not found for {playlistId}", nameof(AddVideoToPlaylistAsync));
            }

            if (existingPlayList.VideoIds.IsNotNullOrEmpty() && existingPlayList.VideoIds.Contains(videoId))
            {
	            return _resultHelper.GetBadRequestFailedResult($"Playlist {playlistId} already contains video ID {videoId}", nameof(AddVideoToPlaylistAsync));
            }

			var updatedVideoIds = await _store.AddVideoToPlaylistAsync(videoId, playlistId);
			return updatedVideoIds != null
				? _resultHelper.GetVideoIdsSuccessResult(updatedVideoIds)
				: _resultHelper.GetNotFoundFailedResult("Failed to Add Video to Playlist", nameof(AddVideoToPlaylistAsync));
        }

        //Remove a video from a playlist if both ids > 0, playlist exists and playlist contains video
        public async Task<MethodResult> RemoveVideoFromPlaylistAsync(int playlistId, int videoId)
        {
            if (videoId <= 0 || playlistId <= 0)
            {
	            return _resultHelper.GetBadRequestFailedResult($"Playlist - {playlistId} or Video ID - {videoId} less than 0", nameof(RemoveVideoFromPlaylistAsync));
            }

            var existingPlayList = await _store.GetPlaylistIfExists(playlistId);
            if (existingPlayList == null)
            {
	            return _resultHelper.GetNotFoundFailedResult($"Playlist not found for {playlistId}", nameof(RemoveVideoFromPlaylistAsync));
            }

            if (!existingPlayList.VideoIds.Contains(videoId))
            {
	            return _resultHelper.GetNotFoundFailedResult($"Playlist {playlistId} does not contain video ID {videoId}", nameof(RemoveVideoFromPlaylistAsync));
            }

            var updatedVideoIds = await _store.RemoveVideoFromPlaylistAsync(videoId, playlistId);
            return updatedVideoIds != null
	            ? _resultHelper.GetVideoIdsSuccessResult(updatedVideoIds)
	            : _resultHelper.GetNotFoundFailedResult("Failed to Remove Video from Playlist", nameof(AddVideoToPlaylistAsync));
        }

        //Get all related videos from a playlist id
        public async Task<List<Video>?> GetAllVideosInPlaylist(int playlistId)
        {
            if (playlistId <= 0)
            {
                _log.LogError("Playlist ID less than 0", nameof(GetAllVideosInPlaylist));
                return null;
            }

            var existingPlayList = await _store.GetPlaylistIfExists(playlistId);
            if (existingPlayList == null)
            {
                _log.LogError($"Playlist not found for {playlistId}", nameof(GetAllVideosInPlaylist));
                return null;
            }

            return existingPlayList.VideoIds.IsNullOrEmpty() ? new List<Video>() : await _store.GetVideos(new VideoPlaylistFilter { Ids = existingPlayList.VideoIds});
        }

        public async Task<List<SimplePlaylist>?> GetAllPlaylistsForVideo(int videoId)
        {
            if(videoId <= 0)
            {
                _log.LogError("Video ID less than 0", nameof(GetAllPlaylistsForVideo));
                return null;
            }

            var video = await _store.GetVideoIfExists(videoId);
            if(video == null)
            {
                _log.LogError($"Video not found for Id {videoId}", nameof(GetAllPlaylistsForVideo));
                return null;
            }

            return await _store.GetSimplePlaylists(new PlaylistFilter { VideoId = videoId });
        }

        public async Task<List<SimplePlaylist>> GetAllSimplePlaylists()
        {
            return await _store.GetSimplePlaylists(new PlaylistFilter());
        }

        public async Task<SimplePlaylist?> GetSimplePlaylistIfExists(int playlistId)
        {
            return (await _store.GetSimplePlaylists(new PlaylistFilter { Id = playlistId})).FirstOrDefault();
        }
    }
}
