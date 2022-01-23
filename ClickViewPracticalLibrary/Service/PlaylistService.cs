using ClickViewPracticalLibrary.Model;
using Microsoft.Extensions.Logging;

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
        public bool AddPlaylist(Playlist playlist)
        {
            if (playlist.ID > 0)
            {
                _log.LogError("Playlist ID greater than 0");
                return false;
            }

            if (string.IsNullOrEmpty(playlist.Name))
            {
                _log.LogError("Playlist name is empty");
                return false;
            }

            if (playlist.VideoIds.Any())
            {
                var videos = GetVideos(new VideoPlaylistFilter { Ids = playlist.VideoIds }).Select(o => o.Id).ToList();
                if (videos.Count != playlist.VideoIds.Count)
                {
                    var invalidIds = playlist.VideoIds.Where(o => !videos.Contains(o));
                    _log.LogError("One of more invalid Video Ids were provided ({invalidIds})",string.Join(',', invalidIds));
                    return false;
                }
            }

            playlist.ID = GetAllPlaylists().Max(o => o.ID) + 1;
            _loader.Playlists.Add(playlist);
            return true;
        }

        //Update an existing playlist. Must have an id greater than 0, a name and all valid video ids
        public bool UpdatePlaylist(Playlist playlist)
        {
            if (playlist.ID <= 0)
            {
                _log.LogError("Playlist ID less than 0 for update");
                return false;
            }

            if (string.IsNullOrEmpty(playlist.Name))
            {
                _log.LogError("Playlist name is empty");
                return false;
            }

            var existingPlayList = GetPlaylistIfExists(playlist.ID);
            if (existingPlayList == null)
            {
                _log.LogError("Playlist not found for {playlist.ID}",playlist.ID);
                return false;
            }

            if (playlist.VideoIds.Any())
            {
                var videos = GetVideos(new VideoPlaylistFilter { Ids = playlist.VideoIds }).Select(o => o.Id).ToList();
                if (videos.Count != playlist.VideoIds.Count)
                {
                    var invalidIds = playlist.VideoIds.Where(o => !videos.Contains(o));
                    _log.LogError("One of more invalid Video Ids were provided ({invalidIds})", string.Join(',', invalidIds));
                    return false;
                }
            }

            existingPlayList.Name = playlist.Name;
            existingPlayList.Description = playlist.Description;
            existingPlayList.VideoIds = playlist.VideoIds;
            return true;
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
