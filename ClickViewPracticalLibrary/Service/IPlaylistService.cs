using ClickViewPracticalLibrary.Model;
using System.Net;

namespace ClickViewPracticalLibrary.Service
{
    public interface IPlaylistService
    {
        public Task<HttpStatusCode> AddPlaylistAsync(Playlist playlist);

        public List<Playlist> GetAllPlaylists();

        public Task<HttpStatusCode> UpdatePlaylistAsync(Playlist playlist);

        public Task<HttpStatusCode> DeletePlaylistAsync(int playlistId);

        public Task<HttpStatusCode> AddVideoToPlaylistAsync(int playlistId, int videoId);

        public Task<HttpStatusCode> RemoveVideoFromPlaylistAsync(int playlistId, int videoId);

        public List<Video> GetAllVideosInPlaylist(int playlistId);
    }
}
