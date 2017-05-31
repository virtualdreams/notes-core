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
		private readonly Settings Options;
		private readonly IMongoClient _client;
		private readonly IMongoDatabase _database;
		public IMongoCollection<Note> Note { get; private set; }
		public IMongoCollection<User> User { get; private set; }
		public IMongoCollection<Token> Token { get; private set; }

		public MongoContext(Settings settings, ILogger<MongoContext> log)
		{
			Log = log;
			Options = settings;

			Log.LogDebug("Set mongo database to '{0}'.", Options.Database);

			var _settings = MongoClientSettings.FromUrl(new MongoUrl(Options.MongoDB));

			// log commands if debug enabled
			if (Log.IsEnabled(LogLevel.Debug))
			{
				Log.LogTrace("MongoDB command log enabled.");
				_settings.ClusterConfigurator = cb =>
				{
					cb.Subscribe<CommandStartedEvent>(e =>
					{
						Log.LogTrace($"{e.CommandName} - {e.Command.ToJson()}");
					});
				};
			}

			_client = new MongoClient(_settings);
			_database = _client.GetDatabase(Options.Database);
			Note = _database.GetCollection<Note>("notes");
			User = _database.GetCollection<User>("user");
			Token = _database.GetCollection<Token>("token");
		}
	}
}