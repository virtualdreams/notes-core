using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace postit.Core.Models
{
	[BsonIgnoreExtraElements]
	public class Postit
	{
		[BsonIdAttribute]
		public ObjectId Id { get; set; }

		[BsonElement("title")]
		public string Title { get; set; }

		[BsonElement("content")]
		public string Content { get; set; }
	}
}
