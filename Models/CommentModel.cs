using MongoDB.Bson;

namespace postit.Models
{
	public class CommentModel
	{
		public ObjectId Id { get; set; }

		public string Age { get; set; }

		public string User { get; set; }

		public string Content { get; set; }
	}
}