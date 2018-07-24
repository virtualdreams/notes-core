using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using notes.Core.Models;
using System;

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
		public void WipeAll()
		{
			Log.LogInformation("Wipe all data.");

			WipeUsers();
			WipeNotes();
			WipeTokens();
		}

		/// <summary>
		/// Wipe all notes from database.
		/// </summary>
		public void WipeNotes()
		{
			Log.LogInformation("Wipe notes.");

			Context.Note.DeleteMany(_ => true);
		}

		/// <summary>
		/// Wipe all users from database.
		/// </summary>
		public void WipeUsers()
		{
			Log.LogInformation("Wipe users.");

			Context.User.DeleteMany(_ => true);
		}

		/// <summary>
		/// Wipe all tokens from database.
		/// </summary>
		public void WipeTokens()
		{
			Log.LogInformation("Wipe tokens");

			Context.Token.DeleteMany(_ => true);
		}

		/// <summary>
		/// (Re)Create all necessary indexes for users, notes and tokens.
		/// </summary>
		public void CreateIndexes()
		{
			Log.LogInformation("Create all indexes.");

			CreateUserIndexes();
			CreateNoteIndexes();
			CreateTokenIndexes();
		}

		/// <summary>
		/// (Re)Create user indexes.
		/// </summary>
		private void CreateUserIndexes()
		{
			var _index = Builders<User>.IndexKeys;
			var _name = _index.Ascending(f => f.Username);

			Log.LogInformation("Create user index.");

			Context.User.Indexes.DropAll();
			Context.User.Indexes.CreateOne(new CreateIndexModel<User>(_name, new CreateIndexOptions { Unique = true }));
		}

		/// <summary>
		/// (Re)Create note indexes.
		/// </summary>
		private void CreateNoteIndexes()
		{
			var _index = Builders<Note>.IndexKeys;
			var _text = _index
				.Text(f => f.Title)
				.Text(f => f.Content)
				.Text(f => f.Notebook)
				.Text(f => f.Tags);

			var _owner_trash = _index
				.Ascending(f => f.Owner)
				.Ascending(f => f.Trash);

			var _id_owner_trash = _index
				.Ascending(f => f.Id)
				.Ascending(f => f.Owner)
				.Ascending(f => f.Trash);

			var _owner_trash_notebook = _index
				.Ascending(f => f.Owner)
				.Ascending(f => f.Trash)
				.Ascending(f => f.Notebook);

			var _id_owner_trash_notebook = _index
				.Ascending(f => f.Id)
				.Ascending(f => f.Owner)
				.Ascending(f => f.Trash)
				.Ascending(f => f.Notebook);

			var _owner_trash_tags = _index
				.Ascending(f => f.Owner)
				.Ascending(f => f.Trash)
				.Ascending(f => f.Tags);

			var _id_owner_trash_tags = _index
				.Ascending(f => f.Id)
				.Ascending(f => f.Owner)
				.Ascending(f => f.Trash)
				.Ascending(f => f.Tags);

			Log.LogInformation("Create note index.");

			Context.Note.Indexes.DropAll();
			Context.Note.Indexes.CreateOne(new CreateIndexModel<Note>(_text, new CreateIndexOptions
			{
				Weights = new BsonDocument
				{
					{ "title", 1 },
					{ "content", 1 },
					{ "notebook", 1 },
					{ "tags", 1 }
				}
			}));
			Context.Note.Indexes.CreateOne(new CreateIndexModel<Note>(_id_owner_trash, new CreateIndexOptions { }));
			Context.Note.Indexes.CreateOne(new CreateIndexModel<Note>(_owner_trash, new CreateIndexOptions { }));
			Context.Note.Indexes.CreateOne(new CreateIndexModel<Note>(_owner_trash_notebook, new CreateIndexOptions { }));
			Context.Note.Indexes.CreateOne(new CreateIndexModel<Note>(_id_owner_trash_notebook, new CreateIndexOptions { }));
			Context.Note.Indexes.CreateOne(new CreateIndexModel<Note>(_id_owner_trash_tags, new CreateIndexOptions { }));
			Context.Note.Indexes.CreateOne(new CreateIndexModel<Note>(_owner_trash_tags, new CreateIndexOptions { }));
		}

		/// <summary>
		/// (Re)Create token indexes.
		/// </summary>
		private void CreateTokenIndexes()
		{
			var _index = Builders<Token>.IndexKeys;
			var _token = _index.Ascending(f => f.Created);

			Log.LogInformation("Create token index.");

			Context.Token.Indexes.DropAll();
			Context.Token.Indexes.CreateOne(new CreateIndexModel<Token>(_token, new CreateIndexOptions { ExpireAfter = new TimeSpan(0, 60, 0) }));
		}
	}
}