using MongoDB.Driver;
using notes.Core.Models;

namespace notes.Core.Services
{
    public class MongoContext
	{
		private readonly IMongoClient _client;
		private readonly IMongoDatabase _database;
		public IMongoCollection<Note> Note { get; private set; }
		public IMongoCollection<User> User { get; private set; }

		public MongoContext(IMongoClient client, string database)
		{
			_client = client;
			_database = client.GetDatabase(database);
			Note = _database.GetCollection<Note>("notes");
			User = _database.GetCollection<User>("user");
		}
	}
}