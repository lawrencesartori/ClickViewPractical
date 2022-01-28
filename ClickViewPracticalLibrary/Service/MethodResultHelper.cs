using Microsoft.Extensions.Logging;
using ClickViewPracticalLibrary.Model;
using System.Net;

namespace ClickViewPracticalLibrary.Service
{
    public class MethodResultHelper
    {
        private readonly ILogger _log;
        public MethodResultHelper(ILogger log)
        {
            _log = log;
        }

        public MethodResult GetBadRequestFailedResult(string message, string nameOfMethod)
        {
            return GetFailedResult(message, nameOfMethod, HttpStatusCode.BadRequest);
        }

        public MethodResult GetNotFoundFailedResult(string message, string nameOfMethod)
        {
            return GetFailedResult(message, nameOfMethod, HttpStatusCode.NotFound);
        }

        public MethodResult GetConflictFailedResult(string message, string nameOfMethod)
        {
            return GetFailedResult(message, nameOfMethod, HttpStatusCode.Conflict);
        }

        public MethodResult GetFailedResult(string message, string nameOfMethod, HttpStatusCode code)
        {
            return GetResultAndMaybeLogError(message, false, nameOfMethod, code);
        }

        public MethodResult GetSuccessResult(string message)
        {
            return GetResultAndMaybeLogError(message, true, string.Empty);
        }

        public MethodResult GetEmptySuccessResult()
        {
            return GetSuccessResult(string.Empty);
        }

        public MethodResult GetPlaylistSuccessResult(SimplePlaylist playlist)
        {
            return new MethodResult(playlist);
        }

        public MethodResult GetVideoIdsSuccessResult(List<int> videoIds)
        {
            return new MethodResult(videoIds);
        }

        public MethodResult GetResultAndMaybeLogError(string message, bool success, string nameOfMethod, HttpStatusCode code = HttpStatusCode.OK)
        {
            var result = new MethodResult(message, success);
            if (!result.IsSuccess)
            {
                _log.LogError(message, nameOfMethod);
            }

            return result;
        }
    }
}
