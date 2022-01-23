
using ClickViewPracticalLibrary.Model;

namespace ClickViewPracticalLibrary.Service
{
    public interface IPlaylistStore
    {

        public List<Playlist> GetPlaylists(VideoPlaylistFilter filter);

        public List<Video> GetVideos(VideoPlaylistFilter filter);

        public Playlist? GetPlaylistIfExists(int playlistId);

        public Video? GetVideoIfExists(int videoId);

        public void AddPlaylist(Playlist playlist);

        public void RemovePlaylist(Playlist playlist);
    }
}
