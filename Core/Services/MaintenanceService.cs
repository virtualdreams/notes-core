using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using notes.Core.Models;

namespace notes.Core.Services
{
	public class MaintenanceService
	{
		private readonly ILogger<MaintenanceService> Log;
		private readonly MongoContext Context;

		public MaintenanceService(ILogger<MaintenanceService> log, MongoContext context)
		{
			Log = log;
			Context = context;
		}

		/// <summary>
		/// Wipe complete database.
		/// </summary>
		public async Task WipeAll()
		{
			Log.LogInformation("Wipe all data.");

			await WipeUsers();
			await WipeNotes();
			await WipeTokens();
		}

		/// <summary>
		/// Wipe all notes from database.
		/// </summary>
		public async Task WipeNotes()
		{
			Log.LogInformation("Wipe notes.");

			await Context.Note.DeleteManyAsync(_ => true);
		}

		/// <summary>
		/// Wipe all users from database.
		/// </summary>
		public async Task WipeUsers()
		{
			Log.LogInformation("Wipe users.");

			await Context.User.DeleteManyAsync(_ => true);
		}

		/// <summary>
		/// Wipe all tokens from database.
		/// </summary>
		public async Task WipeTokens()
		{
			Log.LogInformation("Wipe tokens");

			await Context.Token.DeleteManyAsync(_ => true);
		}

		/// <summary>
		/// (Re)Create all necessary indexes for users, notes and tokens.
		/// </summary>
		public async Task CreateIndexes()
		{
			Log.LogInformation("Create all indexes.");

			await CreateUserIndexes();
			await CreateNoteIndexes();
			await CreateTokenIndexes();
		}

		/// <summary>
		/// (Re)Create user indexes.
		/// </summary>
		private async Task CreateUserIndexes()
		{
			var _index = Builders<User>.IndexKeys;
			var _name = _index.Ascending(f => f.Username);

			Log.LogInformation("Drop user index.");
			await Context.User.Indexes.DropAllAsync();

			Log.LogInformation("Create user index.");
			await Context.User.Indexes.CreateOneAsync(new CreateIndexModel<User>(_name, new CreateIndexOptions { Unique = true }));
		}

		/// <summary>
		/// (Re)Create note indexes.
		/// </summary>
		private async Task CreateNoteIndexes()
		{
			var _index = Builders<Note>.IndexKeys;
			var _text = _index
				.Text(f => f.Title)
				.Text(f => f.Content)
				.Text(f => f.Notebook)
				.Text(f => f.Tags);

			var _trash = _index
				.Ascending(f => f.Trash);

			var _id_trash = _index
				.Ascending(f => f.Id)
				.Ascending(f => f.Trash);

			var _trash_notebook = _index
				.Ascending(f => f.Trash)
				.Ascending(f => f.Notebook);

			var _id_trash_notebook = _index
				.Ascending(f => f.Id)
				.Ascending(f => f.Trash)
				.Ascending(f => f.Notebook);

			var _trash_tags = _index
				.Ascending(f => f.Trash)
				.Ascending(f => f.Tags);

			var _id_trash_tags = _index
				.Ascending(f => f.Id)
				.Ascending(f => f.Trash)
				.Ascending(f => f.Tags);

			Log.LogInformation("Drop note indexes.");
			await Context.Note.Indexes.DropAllAsync();

			Log.LogInformation("Create note indexes.");
			await Context.Note.Indexes.CreateOneAsync(new CreateIndexModel<Note>(_text, new CreateIndexOptions
			{
				Weights = new BsonDocument
				{
					{ "title", 1 },
					{ "content", 1 },
					{ "notebook", 1 },
					{ "tags", 1 }
				}
			}));
			await Context.Note.Indexes.CreateOneAsync(new CreateIndexModel<Note>(_trash, new CreateIndexOptions { }));
			await Context.Note.Indexes.CreateOneAsync(new CreateIndexModel<Note>(_id_trash, new CreateIndexOptions { }));
			await Context.Note.Indexes.CreateOneAsync(new CreateIndexModel<Note>(_trash_notebook, new CreateIndexOptions { }));
			await Context.Note.Indexes.CreateOneAsync(new CreateIndexModel<Note>(_id_trash_notebook, new CreateIndexOptions { }));
			await Context.Note.Indexes.CreateOneAsync(new CreateIndexModel<Note>(_id_trash_tags, new CreateIndexOptions { }));
			await Context.Note.Indexes.CreateOneAsync(new CreateIndexModel<Note>(_trash_tags, new CreateIndexOptions { }));
		}

		/// <summary>
		/// (Re)Create token indexes.
		/// </summary>
		private async Task CreateTokenIndexes()
		{
			var _index = Builders<Token>.IndexKeys;
			var _token = _index.Ascending(f => f.Created);

			Log.LogInformation("Drop token index.");
			await Context.Token.Indexes.DropAllAsync();

			Log.LogInformation("Create token index.");
			await Context.Token.Indexes.CreateOneAsync(new CreateIndexModel<Token>(_token, new CreateIndexOptions { ExpireAfter = new TimeSpan(0, 60, 0) }));
		}
	}
}