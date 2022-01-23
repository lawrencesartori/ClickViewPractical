using ClickViewPracticalLibrary.Model;

namespace ClickViewPracticalLibrary.Service
{
    public class PlaylistStore : IPlaylistStore
    {
        private readonly PlaylistLoader _loader;

        public PlaylistStore(PlaylistLoader loader)
        {
            _loader = loader;
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
            if (filter.Id > 0)
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

        public Playlist? GetPlaylistIfExists(int id)
        {
            return GetPlaylistsAsQueryable(new VideoPlaylistFilter { Id = id }).FirstOrDefault();
        }

        public Video? GetVideoIfExists(int id)
        {
            return GetVideosAsQueryable(new VideoPlaylistFilter { Id = id }).FirstOrDefault();
        }

        public void AddPlaylist(Playlist playlist)
        {
            _loader.Playlists.Add(playlist);
        }

        public void RemovePlaylist(Playlist playlist)
        {
            _loader.Playlists.Remove(playlist);
        }
    }
}
