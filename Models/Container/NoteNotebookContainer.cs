using System.Collections.Generic;

namespace notes.Models
{
	public class NoteNotebookContainer
	{
		public IEnumerable<NoteModel> Notes { get; set; }

		public PageOffset Offset { get; set; }

		public string Notebook { get; set; }
	}
}