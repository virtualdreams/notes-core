using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace postit.Core.Services
{
	public class MongoContext
	{
		private readonly IMongoClient _client;
		private readonly IMongoDatabase _database;
		public IMongoCollection<Postit> Postit { get; private set; }
		public IMongoCollection<User> User { get; private set; }

		public MongoContext(IMongoClient client, string database)
		{
			_client = client;
			_database = client.GetDatabase(database);
			Postit = _database.GetCollection<Postit>("postits");
			User = _database.GetCollection<User>("user");
		}
	}

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

	public class User
	{ }
}