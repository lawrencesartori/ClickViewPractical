
using ClickViewPracticalLibrary.Model;
using System.Net;

namespace ClickViewPracticalLibrary.Service
{
    public interface IPlaylistStore
    {

        public List<Playlist> GetPlaylists(PlaylistFilter filter);

        public List<Video> GetVideos(VideoPlaylistFilter filter);

        public Playlist? GetPlaylistIfExists(int playlistId);

        public Video? GetVideoIfExists(int videoId);

        public Task<HttpStatusCode> AddPlaylistAsync(Playlist playlist);

        public Task<HttpStatusCode> UpdatePlaylistAsync(Playlist playlist);

        public Task<HttpStatusCode> RemovePlaylistAsync(int playlistId);

        public Task<HttpStatusCode> RemoveVideoFromPlaylistAsync(int videoId, int playlistId);

        public Task<HttpStatusCode> AddVideoToPlaylistAsync(int videoId, int playlistId);
    }
}
