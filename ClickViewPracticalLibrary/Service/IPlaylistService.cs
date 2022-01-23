using ClickViewPracticalLibrary.Model;
using System.Net;

namespace ClickViewPracticalLibrary.Service
{
    public interface IPlaylistService
    {
        public HttpStatusCode AddPlaylist(Playlist playlist);

        public List<Playlist> GetAllPlaylists();

        public HttpStatusCode UpdatePlaylist(Playlist playlist);

        public HttpStatusCode DeletePlaylist(int playlistId);
    }
}
