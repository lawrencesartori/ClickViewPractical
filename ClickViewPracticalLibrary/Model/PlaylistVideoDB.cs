using System.Text.Json.Serialization;

namespace ClickViewPracticalLibrary.Model
{
    public class PlaylistVideoDB
    {
        [JsonPropertyName("playlists")]
        public List<Playlist> Playlists { get; set; }

        [JsonPropertyName("videos")]
        public List<Video> Videos { get; set; }
    }
}
