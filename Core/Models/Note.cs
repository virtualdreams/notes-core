using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace notes.Core.Models
{
	[BsonIgnoreExtraElements]
	public class Note
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

		[BsonElement("version")]
		public int Version { get; set; }

		[BsonElement("created")]
		[BsonDateTimeOptions(Kind = DateTimeKind.Local)]
		public DateTime? Created { get; set; }

		[BsonElement("modified")]
		[BsonDateTimeOptions(Kind = DateTimeKind.Local)]
		public DateTime? Modified { get; set; }

		[BsonIgnoreIfNull]
		public double? Score { get; set; }
	}
}
