
using ClickViewPracticalLibrary.Model;
using System.Net;
using ClickViewPracticalLibrary.Model.Filters;

namespace ClickViewPracticalLibrary.Service
{
	public interface IPlaylistStore
    {
	    Task<List<SimplePlaylist>> GetSimplePlaylists(PlaylistFilter filter);

        Task<List<Video>> GetVideos(VideoPlaylistFilter filter);

        Task<Playlist?> GetPlaylistIfExists(int playlistId);

        Task<SimplePlaylist?> GetSimplePlaylistIfExists(int playlistId);

        Task<Video?> GetVideoIfExists(int videoId);

        Task<SimplePlaylist?> AddPlaylistAsync(Playlist playlist);

        Task<SimplePlaylist?> UpdatePlaylistAsync(SimplePlaylist playlist);

        Task<bool> RemovePlaylistAsync(int playlistId);

        Task<List<int>?> RemoveVideoFromPlaylistAsync(int videoId, int playlistId);

        Task<List<int>?> AddVideoToPlaylistAsync(int videoId, int playlistId);
    }
}
