using MongoDB.Bson.Serialization.Attributes;

namespace notes.Core.Models
{
	public class DistinctAndCountResult
	{
		[BsonIdAttribute]
		public string Id { get; set; }

		[BsonElement("count")]
		public int Count { get; set; }
	}
}