using System.Collections.Generic;

namespace postit.Models
{
	public class PostitNotebookContainer
	{
		public IEnumerable<PostitModel> Postits { get; set; }

		public PageOffset Offset { get; set; }

		public string Notebook { get; set; }
	}
}