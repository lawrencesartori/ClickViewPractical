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
                var videos = GetAllVideos().Where(o => playlist.VideoIds.Contains(o.Id)).Select(o => o.Id).ToList();
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


        public List<Playlist> GetAllPlaylists()
        {
            return _loader.Playlists;
        }

        public List<Video> GetAllVideos()
        {
            return _loader.Videos;
        }
    }
}
