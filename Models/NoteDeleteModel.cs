using System.Collections.Generic;
using MongoDB.Bson;
using notes.Helper;

namespace notes.Models
{
	public class NoteDeleteModel
	{
		[ArrayNotEmpty]
		public IEnumerable<ObjectId> Id { get; set; }
	}
}