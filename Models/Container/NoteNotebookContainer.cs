using System.Collections.Generic;

namespace Notes.Models
{
	public class NoteNotebookContainer
	{
		public IEnumerable<NoteModel> Notes { get; set; }

		public Pager Pager { get; set; }

		public string Notebook { get; set; }
	}
}