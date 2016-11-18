using System.Collections.Generic;
using postit.Core.Services;

namespace postit.Models
{
	public class PostitViewContainer
	{
		public PostitModel Postit { get; set; }

		public IEnumerable<CommentModel> Comments { get; set; }
	}
}