using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ClickViewPracticalLibrary.Model
{
    public class MethodResult
    {
        public MethodResult(){}

        public MethodResult(string message, bool success)
        {
	        Message = message;
	        IsSuccess = success;
        }

        public MethodResult(string message, HttpStatusCode code)
        {
            Message = message;
            IsSuccess = false;
            StatusCode = code;
        }

        public MethodResult(SimplePlaylist playlist)
        {
            Playlist = playlist;
            IsSuccess = true;
        }

        public MethodResult(List<int> videoIds)
        {
            VideoIds = videoIds;
            IsSuccess = true;
        }

        public string Message { get; set; } = string.Empty;

        public SimplePlaylist? Playlist { get; set; } = null;

        public List<int>? VideoIds { get; set; } = null;

        public bool IsSuccess { get; set; }

        public HttpStatusCode StatusCode { get; set; }
    }
}
