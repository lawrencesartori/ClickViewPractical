using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace ClickViewPracticalLibrary.Model
{
    public class Video
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("duration")]
        public int Duration { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("datecreated")]
        public DateTime DateCreated { get; set; }
        [JsonPropertyName("thumbnail")]
        public string Thumbnail { get; set; }
    }
}
