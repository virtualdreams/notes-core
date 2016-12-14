using System.Collections.Generic;

namespace notes.Models
{
	public class NoteListContainer
	{
		public IEnumerable<NoteModel> Notes { get; set; }

		public Pager Pager { get; set; }
	}
}