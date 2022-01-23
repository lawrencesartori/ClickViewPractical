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


        [HttpPost]
        [Route("CreatePlaylist")]
        public HttpStatusCode GetPlaylists(Playlist playlist)
        {
            return _playlistService.AddPlaylist(playlist);
        }

        [HttpPost]
        [Route("UpdatePlaylist")]
        public HttpStatusCode UpdatePlaylists(Playlist playlist)
        {
            return _playlistService.UpdatePlaylist(playlist);
        }

        [HttpPost]
        [Route("DeletePlaylist/{playlistId}")]
        public HttpStatusCode RemovePlaylist(int playlistId)
        {
            return _playlistService.DeletePlaylist(playlistId);
        }

        [HttpPost]
        [Route("AddVideoToPlaylist/{videoId}/{playlistId}")]
        public HttpStatusCode AddVideoToPlaylist(int videoId, int playlistId)
        {
            return _playlistService.AddVideoToPlaylist(videoId, playlistId);
        }

        [HttpPost]
        [Route("RemoveVideoFromPlaylist/{videoId}/{playlistId}")]
        public HttpStatusCode RemoveVideoFromPlaylist(int videoId, int playlistId)
        {
            return _playlistService.RemoveVideoFromPlaylist(videoId, playlistId);
        }
    }
}