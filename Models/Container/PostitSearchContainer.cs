using System.Collections.Generic;

namespace postit.Models
{
	public class PostitSearchContainer
	{
		public IEnumerable<PostitModel> Postits { get; set; }

		public PageOffset Offset { get; set; }

		public string Term { get; set; }
	}
}