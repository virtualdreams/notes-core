using System.Collections.Generic;

namespace notes.Models
{
	public class NoteTagContainer
	{
		public IEnumerable<NoteModel> Notes { get; set; }

		public PageOffset Offset { get; set; }

		public string Tag { get; set; }
	}
}