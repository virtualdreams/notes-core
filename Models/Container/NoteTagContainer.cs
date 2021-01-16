using System.Collections.Generic;

namespace Notes.Models
{
	public class NoteTagContainer
	{
		public IEnumerable<NoteModel> Notes { get; set; }

		public Pager Pager { get; set; }

		public string Tag { get; set; }
	}
}