
using ClickViewPracticalLibrary.Model;

namespace ClickViewPracticalLibrary.Service
{
    public interface IPlaylistStore
    {

        public List<Playlist> GetPlaylists(PlaylistFilter filter);

        public List<Video> GetVideos(VideoPlaylistFilter filter);

        public Playlist? GetPlaylistIfExists(int playlistId);

        public Video? GetVideoIfExists(int videoId);

        public Task AddPlaylist(Playlist playlist);

        public Task RemovePlaylist(Playlist playlist);

        public Task SaveChangesAsync();
    }
}
