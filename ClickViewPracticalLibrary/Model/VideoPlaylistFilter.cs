using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickViewPracticalLibrary.Model
{
    public class VideoPlaylistFilter
    {
        public int? Id { get; set; }
        public List<int> Ids { get; set; }
    }

    public class PlaylistFilter : VideoPlaylistFilter
    {
        public int? VideoId { get; set; }
        public List<int> VideoIds { get; set; }
    }
}
