using Microsoft.AspNetCore.Mvc;
using ClickViewPracticalLibrary.Service;
using ClickViewPracticalLibrary.Model;
using System.Net;

namespace ClickViewPractical.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlaylistController : ControllerBase
    {
        private readonly ILogger<PlaylistController> _logger;
        private readonly IPlaylistService _playlistService;

        public PlaylistController(ILogger<PlaylistController> logger, IPlaylistService playlistService)
        {
            _logger = logger;
            _playlistService = playlistService;
        }

        [HttpGet]
        [Route("GetAllPlaylists")]
        public List<Playlist> GetAllPlaylists()
        {
            return _playlistService.GetAllPlaylists();
        }

        [HttpGet]
        [Route("GetAllVideosInPlaylist/{playlistId}")]
        public List<Video> GetAllVideosInPlaylist(int playlistId)
        {
            return _playlistService.GetAllVideosInPlaylist(playlistId);
        }


        [HttpPost]
        [Route("CreatePlaylist")]
        public async Task<HttpStatusCode> GetPlaylists(Playlist playlist)
        {
            return await _playlistService.AddPlaylistAsync(playlist);
        }

        [HttpPost]
        [Route("UpdatePlaylist")]
        public async Task<HttpStatusCode> UpdatePlaylists(Playlist playlist)
        {
            return await _playlistService.UpdatePlaylistAsync(playlist);
        }

        [HttpPost]
        [Route("DeletePlaylist/{playlistId}")]
        public async Task<HttpStatusCode> RemovePlaylist(int playlistId)
        {
            return await _playlistService.DeletePlaylistAsync(playlistId);
        }

        [HttpPost]
        [Route("AddVideoToPlaylist/{videoId}/{playlistId}")]
        public async Task<HttpStatusCode> AddVideoToPlaylist(int videoId, int playlistId)
        {
            return await _playlistService.AddVideoToPlaylistAsync(videoId, playlistId);
        }

        [HttpPost]
        [Route("RemoveVideoFromPlaylist/{videoId}/{playlistId}")]
        public async Task<HttpStatusCode> RemoveVideoFromPlaylist(int videoId, int playlistId)
        {
            return await _playlistService.RemoveVideoFromPlaylistAsync(videoId, playlistId);
        }
    }
}