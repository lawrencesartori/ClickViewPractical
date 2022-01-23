using Microsoft.AspNetCore.Mvc;
using ClickViewPracticalLibrary.Service;
using ClickViewPracticalLibrary.Model;

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
        public bool GetPlaylists(Playlist playlist)
        {
            return _playlistService.AddPlaylist(playlist);
        }
    }
}