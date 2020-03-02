using System;

namespace notes.Models
{
	public class RevisionModel
	{
		public int Id { get; set; }

		public DateTime Dt { get; set; }

		public int NoteId { get; set; }

		public string Title { get; set; }

		public string Content { get; set; }

		// public IEnumerable<string> Tags { get; set; }

		// public string TagsString { get; set; }

		public string Notebook { get; set; }

		public bool Trash { get; set; }

		public DateTime Created { get; set; }

		public DateTime Modified { get; set; }
	}
}