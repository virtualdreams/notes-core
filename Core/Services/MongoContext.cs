using Microsoft.Extensions.Options;
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

		public MongoContext(IOptions<Settings> settings)
		{
			_client = new MongoClient(settings.Value.MongoDB);
			_database = _client.GetDatabase(settings.Value.Database);
			Note = _database.GetCollection<Note>("notes");
			User = _database.GetCollection<User>("user");
		}
	}
}