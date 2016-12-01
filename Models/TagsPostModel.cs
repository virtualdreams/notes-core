using MongoDB.Bson;

namespace notes.Models
{
	public class TagsPostModel
	{
		public ObjectId Id { get; set; }

		public string Tags { get; set; }
	}
}