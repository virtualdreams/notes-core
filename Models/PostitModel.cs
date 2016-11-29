using MongoDB.Bson;

namespace postit.Models
{
	public class PostitModel
	{
		public ObjectId Id { get; set; }

		public string Age { get; set; }

		public string Title { get; set; }

		public string Content { get; set; }

		public string Notebook { get; set; }

		public bool Trash { get; set; }
	}
}