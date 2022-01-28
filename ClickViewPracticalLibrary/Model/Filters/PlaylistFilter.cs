namespace ClickViewPracticalLibrary.Model.Filters
{
	public class PlaylistFilter : VideoPlaylistFilter
	{
		public int? VideoId { get; set; }
		public List<int> VideoIds { get; set; }
	}
}
