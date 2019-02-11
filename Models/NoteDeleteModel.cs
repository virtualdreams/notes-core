using System.Collections.Generic;
using notes.Helper;

namespace notes.Models
{
	public class NoteDeleteModel
	{
		[ArrayNotEmpty]
		public IEnumerable<int> Id { get; set; }
	}
}