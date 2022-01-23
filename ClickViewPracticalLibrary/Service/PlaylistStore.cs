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

        public List<Playlist> GetPlaylists(PlaylistFilter filter)
        {
            return GetPlaylistsAsQueryable(filter).ToList();
        }

        public List<Video> GetVideos(VideoPlaylistFilter filter)
        {
            return GetVideosAsQueryable(filter).ToList();
        }

        private IQueryable<Video> GetVideosAsQueryable(VideoPlaylistFilter filter)
        {
            var vids = _loader.PlaylistVideoDB.Videos.AsQueryable();
            if (filter.Id != null && filter.Id > 0)
            {
                vids = vids.Where(o => o.Id == filter.Id);
            }
            if (filter.Ids != null && filter.Ids.Any())
            {
                vids = vids.Where(o => filter.Ids.Contains(o.Id));
            }
            return vids;
        }

        private IQueryable<Playlist> GetPlaylistsAsQueryable(PlaylistFilter filter)
        {
            var playLists = _loader.PlaylistVideoDB.Playlists.AsQueryable();
            if (filter.Id != null && filter.Id > 0)
            {
                playLists = playLists.Where(o => o.ID == filter.Id);
            }
            
            if (filter.Ids != null && filter.Ids.Any())
            {
                playLists = playLists.Where(o => filter.Ids.Contains(o.ID));
            }
            
            if(filter.VideoId != null && filter.VideoId > 0)
            {
                playLists = playLists.Where(o => o.VideoIds.Contains(filter.VideoId.Value));
            }

            return playLists;
        }

        public Playlist? GetPlaylistIfExists(int id)
        {
            return GetPlaylistsAsQueryable(new PlaylistFilter { Id = id }).FirstOrDefault();
        }

        public Video? GetVideoIfExists(int id)
        {
            return GetVideosAsQueryable(new VideoPlaylistFilter { Id = id }).FirstOrDefault();
        }

        public async Task AddPlaylist(Playlist playlist)
        {
            _loader.PlaylistVideoDB.Playlists.Add(playlist);
            await SaveChangesAsync();
        }

        public async Task RemovePlaylist(Playlist playlist)
        {
            _loader.PlaylistVideoDB.Playlists.Remove(playlist);
            await SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _loader.SaveChangesAsync();
        }
    }
}
