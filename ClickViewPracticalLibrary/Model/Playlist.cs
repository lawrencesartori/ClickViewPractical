using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace ClickViewPracticalLibrary.Model
{
    public class Playlist : SimplePlaylist
    {
	    [JsonPropertyName("videoIds")]
        public List<int> VideoIds { get; set; } = new List<int>();  
    }
}
