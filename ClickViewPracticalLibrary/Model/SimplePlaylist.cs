using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ClickViewPracticalLibrary.Model
{
    public class SimplePlaylist
    {
		public SimplePlaylist(){}

		public SimplePlaylist(Playlist pl)
		{
			Name = pl.Name;
			Description = pl.Description;
			Id = pl.Id;
		}

		[JsonPropertyName("name")] 
		public string Name { get; set; } = "";

	    [JsonPropertyName("description")] 
	    public string Description { get; set; } = "";

	    [JsonPropertyName("id")]
	    public int Id { get; set; }
    }
}
