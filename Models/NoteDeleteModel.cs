using System.Collections.Generic;
using MongoDB.Bson;

namespace notes.Models
{
	public class NoteDeleteModel
	{
		public IEnumerable<ObjectId> Id { get; set; }
	}
}