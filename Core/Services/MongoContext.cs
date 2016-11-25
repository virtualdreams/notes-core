using MongoDB.Driver;
using postit.Core.Models;

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
}