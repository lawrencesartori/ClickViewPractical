using ClickViewPracticalLibrary.Model;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ClickViewPracticalLibrary.Service
{
    public class PlaylistService : IPlaylistService
    {
        private readonly PlaylistLoader _loader;
        private readonly ILogger<PlaylistService> _log;

        public PlaylistService(PlaylistLoader loader, ILogger<PlaylistService> logger)
        {
            _loader = loader;
            _log = logger;
        }


        //Adds a playlist if it has all valid video ids, id <=0 and a name
        public HttpStatusCode AddPlaylist(Playlist playlist)
        {
            if (playlist.ID > 0)
            {
                _log.LogError("Playlist ID greater than 0");
                return HttpStatusCode.BadRequest;
            }

            if (string.IsNullOrEmpty(playlist.Name))
            {
                _log.LogError("Playlist name is empty");
                return HttpStatusCode.BadRequest;
            }

            if (playlist.VideoIds.Any())
            {
                var videos = GetVideos(new VideoPlaylistFilter { Ids = playlist.VideoIds }).Select(o => o.Id).ToList();
                if (videos.Count != playlist.VideoIds.Count)
                {
                    var invalidIds = playlist.VideoIds.Where(o => !videos.Contains(o));
                    _log.LogError("One of more invalid Video Ids were provided ({invalidIds})",string.Join(',', invalidIds));
                    return HttpStatusCode.BadRequest;
                }
            }

            playlist.ID = GetAllPlaylists().Max(o => o.ID) + 1;
            _loader.Playlists.Add(playlist);
            return HttpStatusCode.OK;
        }

        //Update an existing playlist. Must have an id greater than 0, a name and all valid video ids
        public HttpStatusCode UpdatePlaylist(Playlist playlist)
        {
            if (playlist.ID <= 0)
            {
                _log.LogError("Playlist ID less than 0 for update");
                return HttpStatusCode.BadRequest;
            }

            if (string.IsNullOrEmpty(playlist.Name))
            {
                _log.LogError("Playlist name is empty");
                return HttpStatusCode.BadRequest;
            }

            var existingPlayList = GetPlaylistIfExists(playlist.ID);
            if (existingPlayList == null)
            {
                _log.LogError("Playlist not found for {playlist.ID}",playlist.ID);
                return HttpStatusCode.NotFound;
            }

            if (playlist.VideoIds.Any())
            {
                var videos = GetVideos(new VideoPlaylistFilter { Ids = playlist.VideoIds }).Select(o => o.Id).ToList();
                if (videos.Count != playlist.VideoIds.Count)
                {
                    var invalidIds = playlist.VideoIds.Where(o => !videos.Contains(o));
                    _log.LogError("One of more invalid Video Ids were provided ({invalidIds})", string.Join(',', invalidIds));
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
                _log.LogError("Playlist ID less than 0 for deletion");
                return HttpStatusCode.BadRequest;
            }

            var existingPlaylist = GetPlaylistIfExists(playlistId);
            if(existingPlaylist == null)
            {
                _log.LogError("Playlist not found for id {playlistId} for deletion", playlistId);
                return HttpStatusCode.NotFound;
            }

            _loader.Playlists.Remove(existingPlaylist);
            return HttpStatusCode.OK;
        }


        //Will add video to playlist if both ids > 0, both exist & playlist doesn't already contain video.
        public HttpStatusCode AddVideoToPlaylist(int videoId, int playlistId)
        {
            if(videoId <= 0 || playlistId <= 0)
            {
                _log.LogError("Playlist - {playlistId} or Video ID - {videoId} less than 0 for adding to playlist",playlistId,videoId);
                return HttpStatusCode.BadRequest;
            }

            var videoToAdd = GetVideoIfExists(videoId);
            if (videoToAdd == null)
            {
                _log.LogError("Video not found for {videoId}", videoId);
                return HttpStatusCode.NotFound;
            }

            var existingPlayList = GetPlaylistIfExists(playlistId);
            if (existingPlayList == null)
            {
                _log.LogError("Playlist not found for {playlistId}", playlistId);
                return HttpStatusCode.NotFound;
            }

            if (existingPlayList.VideoIds.Contains(videoId))
            {
                _log.LogError("Playlist {playlistId} already contains video ID {videoId}", playlistId, videoId);
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
                _log.LogError("Playlist - {playlistId} or Video ID - {videoId} less than 0 for removal from playlist", playlistId, videoId);
                return HttpStatusCode.BadRequest;
            }

            var existingPlayList = GetPlaylistIfExists(playlistId);
            if (existingPlayList == null)
            {
                _log.LogError("Playlist not found for {playlistId}", playlistId);
                return HttpStatusCode.NotFound;
            }

            if (!existingPlayList.VideoIds.Contains(videoId))
            {
                _log.LogError("Playlist {playlistId} does not contain video ID {videoId}", playlistId, videoId);
                return HttpStatusCode.BadRequest;
            }

            existingPlayList.VideoIds.Remove(videoId);
            return HttpStatusCode.OK;
        }

        public List<Playlist> GetAllPlaylists()
        {
            return GetPlaylists(new VideoPlaylistFilter());
        }

        public List<Video> GetAllVideos()
        {
            return GetVideos(new VideoPlaylistFilter()).ToList();
        }

        public List<Playlist> GetPlaylists(VideoPlaylistFilter filter)
        {
            return GetPlaylistsAsQueryable(filter).ToList();
        }

        public List<Video> GetVideos(VideoPlaylistFilter filter)
        {
            return GetVideosAsQueryable(filter).ToList();
        }

        private IQueryable<Video> GetVideosAsQueryable(VideoPlaylistFilter filter)
        {
            var vids = _loader.Videos.AsQueryable();
            if(filter.Id > 0)
            {
                vids = vids.Where(o => o.Id == filter.Id);
            }
            if (filter.Ids != null && filter.Ids.Any())
            {
                vids = vids.Where(o => filter.Ids.Contains(o.Id));
            }
            return vids;
        }

        private IQueryable<Playlist> GetPlaylistsAsQueryable(VideoPlaylistFilter filter)
        {
            var playLists = _loader.Playlists.AsQueryable();
            if (filter.Id > 0)
            {
                playLists = playLists.Where(o => o.ID == filter.Id);
            }
            if (filter.Ids != null && filter.Ids.Any())
            {
                playLists = playLists.Where(o => filter.Ids.Contains(o.ID));
            }
            return playLists;
        }

        private Playlist? GetPlaylistIfExists(int id)
        {
            return GetPlaylistsAsQueryable(new VideoPlaylistFilter { Id = id}).FirstOrDefault();
        }

        private Video? GetVideoIfExists(int id)
        {
            return GetVideosAsQueryable(new VideoPlaylistFilter { Id = id }).FirstOrDefault();
        }
    }
}
