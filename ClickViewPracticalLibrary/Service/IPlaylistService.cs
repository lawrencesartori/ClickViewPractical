using ClickViewPracticalLibrary.Model;
using System.Net;

namespace ClickViewPracticalLibrary.Service
{
	public interface IPlaylistService
    {
        Task<MethodResult> AddPlaylistAsync(SimplePlaylist playlist);

        Task<List<SimplePlaylist>> GetAllSimplePlaylists();

        Task<MethodResult> UpdatePlaylistAsync(SimplePlaylist playlist);

        Task<MethodResult> DeletePlaylistAsync(int playlistId);

        Task<MethodResult> AddVideoToPlaylistAsync(int playlistId, int videoId);

        Task<MethodResult> RemoveVideoFromPlaylistAsync(int playlistId, int videoId);

        Task<List<Video>?> GetAllVideosInPlaylist(int playlistId);

        Task<List<SimplePlaylist>?> GetAllPlaylistsForVideo(int video);
    }
}
