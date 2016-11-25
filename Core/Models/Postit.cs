using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace postit.Core.Models
{
	[BsonIgnoreExtraElements]
	public class Postit
	{
		[BsonIdAttribute]
		public ObjectId Id { get; set; }

		[BsonElement("owner")]
		public ObjectId Owner { get; set; }

		[BsonElement("title")]
		public string Title { get; set; }

		[BsonElement("content")]
		public string Content { get; set; }

		[BsonElement("tags")]
		public string[] Tags { get; set; }

		[BsonElement("notebook")]
		public string Notebook { get; set; }

		[BsonElement("trash")]
		public bool Trash { get; set; }

		[BsonElement("shared")]
		public bool Shared { get; set; }
	}
}
