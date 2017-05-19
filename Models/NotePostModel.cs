using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;

namespace notes.Models
{
	public class NotePostModel
	{

		public ObjectId Id { get; set; }

		[Required]
		public string Title { get; set; }

		[Required]
		public string Content { get; set; }

		public string Notebook { get; set; }

		public string Tags { get; set; }
	}
}