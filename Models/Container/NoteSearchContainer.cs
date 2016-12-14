using System.Collections.Generic;

namespace notes.Models
{
	public class NoteSearchContainer
	{
		public IEnumerable<NoteModel> Notes { get; set; }

		public Pager Pager { get; set; }

		public string Term { get; set; }
	}
}