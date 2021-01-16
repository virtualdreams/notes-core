using System.Collections.Generic;

namespace Notes.Models
{
	public class NoteSearchContainer
	{
		public IEnumerable<NoteModel> Notes { get; set; }

		public Pager Pager { get; set; }

		public string Term { get; set; }
	}
}