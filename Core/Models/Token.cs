using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace notes.Core.Models
{
	[BsonIgnoreExtraElements]
	public class Token
	{
		[BsonIdAttribute]
		public ObjectId Id { get; set; }

		[BsonDateTimeOptions(Kind = DateTimeKind.Local)]
		[BsonElement("created")]
		public DateTime Created { get; set; }

		[BsonElement("nonce")]
		public string Nonce { get; set; }

		[BsonElement("user")]
		public ObjectId User { get; set; }
	}
}