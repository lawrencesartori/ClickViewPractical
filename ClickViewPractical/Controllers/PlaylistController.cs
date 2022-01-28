using Microsoft.AspNetCore.Mvc;
using ClickViewPracticalLibrary.Service;
using ClickViewPracticalLibrary.Model;
using System.Net;
using ClickViewPracticalLibrary;

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

        [HttpGet,Route("GetAllPlaylists")]
        public async Task<IActionResult> GetAllPlaylists()
        {
            return Ok(await _playlistService.GetAllSimplePlaylists());
        }

        
        [HttpGet, Route("GetAllVideosInPlaylist/{playlistId}")]
        public async Task<IActionResult> GetAllVideosInPlaylist(int playlistId)
        {
            var playlists = await _playlistService.GetAllVideosInPlaylist(playlistId);
            if(playlists == null)
            {
                return NotFound($"Playlist not found");
            }
            return Ok(playlists);
        }


        [HttpGet,Route("GetAllPlaylistsForVideo/{videoId}")]
        public async Task<IActionResult> GetAllPlaylistsForVideo(int videoId)
        {
            var results = await _playlistService.GetAllPlaylistsForVideo(videoId);
            if(results == null)
            {
                return NotFound("Video not found");
            }
            return Ok(results);
        }


        
        [HttpPost,Route("CreatePlaylist")]
        public async Task<IActionResult> CreatePlaylist(SimplePlaylist playlist)
        {
            var result = await _playlistService.AddPlaylistAsync(playlist);
            return ReturnResult(result);
        }

        
        [HttpPut,Route("UpdatePlaylist")]
        public async Task<IActionResult> UpdatePlaylists(SimplePlaylist playlist)
        {
            var result = await _playlistService.UpdatePlaylistAsync(playlist);
            return ReturnResult(result);
        }


        [HttpDelete,Route("DeletePlaylist/{playlistId}")]
        public async Task<IActionResult> RemovePlaylist(int playlistId)
        {
            var result = await _playlistService.DeletePlaylistAsync(playlistId);
            return ReturnResult(result);
        }

        [HttpPut,Route("AddVideoToPlaylist/{playlistId}/{videoId}")]
        public async Task<IActionResult> AddVideoToPlaylist(int playlistId, int videoId)
        {
            var result = await _playlistService.AddVideoToPlaylistAsync(playlistId,videoId);
            return ReturnResult(result);
        }

        [HttpDelete,Route("RemoveVideoFromPlaylist/{playlistId}/{videoId}")]
        public async Task<IActionResult> RemoveVideoFromPlaylist(int playlistId, int videoId)
        {
	        var result = await _playlistService.RemoveVideoFromPlaylistAsync(playlistId, videoId);
	        return ReturnResult(result);
        }

        private IActionResult ReturnResult(MethodResult result)
        {
	        if (result.IsSuccess)
	        {
                if(result.Playlist != null)
                {
                    return Ok(result.Playlist);
                }
                if (result.VideoIds != null)
                {
                    return Ok(result.VideoIds);
                }
                return result.Message.IsNullOrEmpty() ? NoContent() : Ok(result.Message);
	        }

            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(result.Message);
            }
            else
            {
                return BadRequest(result.Message);

            }
        }
    }
}