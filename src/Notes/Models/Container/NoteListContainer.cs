using System.Collections.Generic;

namespace Notes.Models
{
	public class NoteListContainer
	{
		public IEnumerable<NoteModel> Notes { get; set; }

		public Pager Pager { get; set; }
	}
}