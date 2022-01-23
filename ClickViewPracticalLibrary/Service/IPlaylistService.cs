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

        public HttpStatusCode AddVideoToPlaylist(int playlistId, int videoId);

        public HttpStatusCode RemoveVideoFromPlaylist(int playlistId, int videoId);

        public List<Video> GetAllVideosInPlaylist(int playlistId);
    }
}
