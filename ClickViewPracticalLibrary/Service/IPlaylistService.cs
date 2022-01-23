using ClickViewPracticalLibrary.Model;

namespace ClickViewPracticalLibrary.Service
{
    public interface IPlaylistService
    {
        public bool AddPlaylist(Playlist playlist);

        public List<Playlist> GetAllPlaylists();

        public bool UpdatePlaylist(Playlist playlist);
    }
}
