using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver.Core.Events;
using MongoDB.Driver;
using notes.Core.Models;

namespace notes.Core.Services
{
    public class MongoContext
	{
		private readonly ILogger<MongoContext> Log;
		private readonly IOptions<Settings> Settings;
		private readonly IMongoClient _client;
		private readonly IMongoDatabase _database;
		public IMongoCollection<Note> Note { get; private set; }
		public IMongoCollection<User> User { get; private set; }
		public IMongoCollection<Token> Token { get; private set; }

		public MongoContext(IOptions<Settings> settings, ILogger<MongoContext> log)
		{
			Log = log;
			Settings = settings;

			Log.LogDebug("Set mongo database to '{0}'.", Settings.Value.Database);

			var _settings = MongoClientSettings.FromUrl(new MongoUrl(Settings.Value.MongoDB));

			// log commands if debug enabled
			if(Log.IsEnabled(LogLevel.Debug))
			{
				Log.LogDebug("MongoDB command log enabled.");
				_settings.ClusterConfigurator = cb =>
				{
					cb.Subscribe<CommandStartedEvent>(e =>
					{
						Log.LogDebug($"{e.CommandName} - {e.Command.ToJson()}");
					});
				};
			}

			_client = new MongoClient(_settings);
			_database = _client.GetDatabase(Settings.Value.Database);
			Note = _database.GetCollection<Note>("notes");
			User = _database.GetCollection<User>("user");
			Token = _database.GetCollection<Token>("token");
		}
	}
}