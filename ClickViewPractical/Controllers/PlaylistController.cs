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
        private readonly PlaylistLoader _playListLoader;

        public PlaylistController(ILogger<PlaylistController> logger, PlaylistLoader playlistLoader)
        {
            _logger = logger;
            _playListLoader = playlistLoader;
        }

        [HttpGet]
        [Route("GetPlaylists")]
        public List<Playlist> GetPlaylists()
        {
            return _playListLoader.Playlists;
        }

        [HttpGet]
        [Route("GetVideos")]
        public List<Video> GetVideos()
        {
            return _playListLoader.Videos;
        }
    }
}