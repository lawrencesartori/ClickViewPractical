using ClickViewPracticalLibrary.Model;
using System.Net;

namespace ClickViewPracticalLibrary.Service
{
    public interface IPlaylistService
    {
        public Task<HttpStatusCode> AddPlaylistAsync(Playlist playlist);

        public List<Playlist> GetAllPlaylists();

        public Task<HttpStatusCode> UpdatePlaylistAsync(Playlist playlist);

        public Task<HttpStatusCode> DeletePlaylistAsync(VideoPlaylistApiModel model);

        public Task<HttpStatusCode> AddVideoToPlaylistAsync(VideoPlaylistApiModel model);

        public Task<HttpStatusCode> RemoveVideoFromPlaylistAsync(VideoPlaylistApiModel model);

        public List<Video> GetAllVideosInPlaylist(int playlistId);

        public List<Playlist> GetAllPlaylistsForVideo(int video);
    }
}
