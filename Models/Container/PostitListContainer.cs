using System.Collections.Generic;

namespace postit.Models
{
	public class PostitListContainer
	{
		public IEnumerable<PostitModel> Postits { get; set; }

		public PageOffset Offset { get; set; }
	}
}