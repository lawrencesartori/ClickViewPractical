using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ClickViewPracticalLibrary.Model
{
    public class VideoPlaylistApiModel
    {
        [JsonPropertyName("vid")]
        public int? videoId { get; set; }
        [JsonPropertyName("pid")]
        public int playlistId { get; set; }
    }
}
