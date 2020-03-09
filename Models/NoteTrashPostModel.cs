using System.Collections.Generic;
using notes.Extensions;

namespace notes.Models
{
	public class NoteTrashPostModel
	{
		[ArrayNotEmpty]
		public IEnumerable<int> Id { get; set; }
	}
}