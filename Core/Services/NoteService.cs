using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using notes.Core.Models;

namespace notes.Core.Services
{
	public class NoteService
	{
		private readonly ILogger<NoteService> Log;
		private readonly MongoContext Context;

		public NoteService(ILogger<NoteService> log, MongoContext context)
		{
			Log = log;
			Context = context;
		}

		/// <summary>
		/// Get a list of notes.
		/// </summary>
		/// <param name="next">The next id.</param>
		/// <param name="trashed">Request trashed items or not.</param>
		/// <param name="limit">Limit the result.</param>
		/// <returns></returns>
		public async Task<IEnumerable<Note>> GetNotes(ObjectId next, bool trashed, int limit)
		{
			var _filter = Builders<Note>.Filter;
			var _active = _filter.Eq(f => f.Trash, trashed);
			var _next = _filter.Lt(f => f.Id, next);

			var _sort = Builders<Note>.Sort;
			var _order = _sort.Descending(f => f.Id);

			var _query = _active;

			if (next != ObjectId.Empty)
				_query &= _next;

			Log.LogInformation($"Get all notes.");

			var _result = await Context.Note
				.Find(_query)
				.Sort(_order)
				.Limit(limit)
				.ToListAsync();

			return _result;
		}

		/// <summary>
		/// Get a list of notes by notebook name.
		/// </summary>
		/// <param name="notebook">The notebook name.</param>
		/// <param name="next">The next cursor.</param>
		/// <param name="limit">Limit the result.</param>
		/// <returns></returns>
		public async Task<IEnumerable<Note>> GetByNotebook(string notebook, ObjectId next, int limit)
		{
			notebook = notebook?.Trim();

			var _filter = Builders<Note>.Filter;
			var _notebook = _filter.Eq(f => f.Notebook, notebook);
			var _active = _filter.Eq(f => f.Trash, false);
			var _next = _filter.Lt(f => f.Id, next);

			var _sort = Builders<Note>.Sort;
			var _order = _sort.Descending(f => f.Id);

			var _query = _notebook & _active;

			if (next != ObjectId.Empty)
				_query &= _next;

			Log.LogInformation($"Get notes by notebook '{notebook}'.");

			var _result = await Context.Note
				.Find(_query)
				.Sort(_order)
				.Limit(limit)
				.ToListAsync();

			return _result;
		}

		/// <summary>
		/// Get a list of notes by tag.
		/// </summary>
		/// <param name="tag">The tag name.</param>
		/// <param name="next">The next cursor.</param>
		/// <param name="limit">Limit the result.</param>
		/// <returns></returns>
		public async Task<IEnumerable<Note>> GetByTag(string tag, ObjectId next, int limit)
		{
			tag = tag?.Trim();

			var _filter = Builders<Note>.Filter;
			var _tag = _filter.AnyEq("Tags", tag);
			var _active = _filter.Eq(f => f.Trash, false);
			var _next = _filter.Lt(f => f.Id, next);

			var _sort = Builders<Note>.Sort;
			var _order = _sort.Descending(f => f.Id);

			var _query = _tag & _active;

			if (next != ObjectId.Empty)
				_query &= _next;

			Log.LogInformation($"Get notes by tag '{tag}'.");

			var _result = await Context.Note
				.Find(_query)
				.Sort(_order)
				.Limit(limit)
				.ToListAsync();

			return _result;
		}

		/// <summary>
		/// Get a note by id.
		/// </summary>
		/// <param name="note">The note id.</param>
		/// <returns></returns>
		public async Task<Note> GetById(ObjectId note)
		{
			var _filter = Builders<Note>.Filter;
			var _id = _filter.Eq(f => f.Id, note);

			var _query = _id;

			Log.LogInformation($"Get note by id {note}.");

			var _result = await Context.Note
				.Find(_query)
				.SingleOrDefaultAsync();

			return _result;
		}

		/// <summary>
		/// Get mostly used notebooks.
		/// </summary>
		/// <param name="limit">Limit result to n items.</param>
		/// <returns>A list of notebook.</returns>
		public async Task<IEnumerable<DistinctAndCountResult>> GetMostUsedNotebooks(int limit = 10)
		{
			var _filter = Builders<Note>.Filter;
			var _size = _filter.Size(f => f.Notebook, 0);
			var _not = _filter.Not(_size);
			var _active = _filter.Eq(f => f.Trash, false);

			var _query = _active & _not;

			Log.LogDebug($"Get most used notebooks.");

			var _result = await Context.Note
				.Aggregate()
				.Match(_query)
				.Group(new BsonDocument { { "_id", "$notebook" }, { "count", new BsonDocument { { "$sum", 1 } } } })
				.Match(new BsonDocument { { "count", new BsonDocument { { "$gte", 1 } } }, { "_id", new BsonDocument { { "$nin", new BsonArray { BsonNull.Value, "" } } } } })
				.Sort(new BsonDocument { { "count", -1 } })
				.Limit(limit)
				.As<DistinctAndCountResult>()
				.ToListAsync();

			return _result;
		}

		/// <summary>
		/// Get a list of all notebooks.
		/// </summary>
		/// <returns>A list of notebook.</returns>
		public async Task<IEnumerable<DistinctAndCountResult>> GetNotebooks()
		{
			var _filter = Builders<Note>.Filter;
			var _size = _filter.Size(f => f.Notebook, 0);
			var _not = _filter.Not(_size);
			var _active = _filter.Eq(f => f.Trash, false);

			var _query = _active & _not;

			Log.LogInformation($"Get full list of notebooks.");

			var _result = await Context.Note
				.Aggregate()
				.Match(_query)
				.Group(new BsonDocument { { "_id", "$notebook" }, { "count", new BsonDocument { { "$sum", 1 } } } })
				.Match(new BsonDocument { { "count", new BsonDocument { { "$gte", 1 } } }, { "_id", new BsonDocument { { "$nin", new BsonArray { BsonNull.Value, "" } } } } })
				.Sort(new BsonDocument { { "_id", 1 } })
				.As<DistinctAndCountResult>()
				.ToListAsync();

			return _result;
		}

		/// <summary>
		/// Get mostly used tags.
		/// </summary>
		/// <param name="limit">Limit result to n items.</param>
		/// <returns>A list of tags.</returns>
		public async Task<IEnumerable<DistinctAndCountResult>> GetMostUsedTags(int limit = 10)
		{
			var _filter = Builders<Note>.Filter;
			var _size = _filter.Size(f => f.Tags, 0);
			var _not = _filter.Not(_size);
			var _active = _filter.Eq(f => f.Trash, false);

			var _query = _active & _not;

			Log.LogDebug($"Get most used tags.");

			var _result = await Context.Note
				.Aggregate()
				.Match(_query)
				.Unwind(f => f.Tags)
				.Group(new BsonDocument { { "_id", "$tags" }, { "count", new BsonDocument { { "$sum", 1 } } } })
				.Match(new BsonDocument { { "count", new BsonDocument { { "$gte", 1 } } }, { "_id", new BsonDocument { { "$nin", new BsonArray { BsonNull.Value, "" } } } } })
				.Sort(new BsonDocument { { "count", -1 } })
				.Limit(limit)
				.As<DistinctAndCountResult>()
				.ToListAsync();

			return _result;
		}

		// <summary>
		/// Get a list of all tags,
		/// </summary>
		/// <returns>A list of tags.</returns>
		public async Task<IEnumerable<DistinctAndCountResult>> GetTags()
		{
			var _filter = Builders<Note>.Filter;
			var _size = _filter.Size(f => f.Tags, 0);
			var _not = _filter.Not(_size);
			var _active = _filter.Eq(f => f.Trash, false);

			var _query = _active & _not;

			Log.LogInformation($"Get full list of tags.");

			var _result = await Context.Note
				.Aggregate()
				.Match(_query)
				.Unwind(f => f.Tags)
				.Group(new BsonDocument { { "_id", "$tags" }, { "count", new BsonDocument { { "$sum", 1 } } } })
				.Match(new BsonDocument { { "count", new BsonDocument { { "$gte", 1 } } }, { "_id", new BsonDocument { { "$nin", new BsonArray { BsonNull.Value, "" } } } } })
				.Sort(new BsonDocument { { "_id", 1 } })
				.As<DistinctAndCountResult>()
				.ToListAsync();

			return _result;
		}

		/// <summary>
		/// Create a new note.
		/// </summary>
		/// <param name="user">The user who create the note.</param>
		/// <param name="title">The note title.</param>
		/// <param name="content">The note content.</param>
		/// <param name="notebook">The notebook name.</param>
		/// <param name="tags">The note tags.</param>
		/// <returns></returns>
		public async Task<ObjectId> Create(ObjectId user, string title, string content, string notebook, string tags)
		{
			var _tags = tags?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(s => s.Trim())
				.Where(s => !String.IsNullOrEmpty(s))
				.Distinct()
				.ToArray();

			var _note = new Note
			{
				Owner = user,
				Title = title?.Trim(),
				Content = content?.Trim(),
				Tags = _tags,
				Notebook = notebook?.Trim(),
				Trash = false,
				Version = 1,
				Created = DateTime.UtcNow
			};

			await Context.Note.InsertOneAsync(_note);

			Log.LogInformation($"Create new note with id {_note.Id} for user {user}.");

			return _note.Id;
		}

		/// <summary>
		/// Update a note.
		/// </summary>
		/// <param name="note">The note id.</param>
		/// <param name="user">The user who update the note</param>
		/// <param name="title">The note title.</param>
		/// <param name="content">The note content.</param>
		/// <param name="notebook">The note notebook.</param>
		/// <param name="tags">The note tags.</param>
		/// <returns></returns>
		public async Task<bool> Update(ObjectId note, ObjectId user, string title, string content, string notebook, string tags)
		{
			var _tags = tags?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(s => s.Trim())
				.Where(s => !String.IsNullOrEmpty(s))
				.Distinct()
				.ToArray();

			var _filter = Builders<Note>.Filter;
			var _id = _filter.Eq(f => f.Id, note);
			//var _user = _filter.Eq(f => f.Owner, user);

			var _query = _id;

			var _update = Builders<Note>.Update;
			var _set = _update
						.Set(f => f.Title, title?.Trim())
						.Set(f => f.Content, content?.Trim())
						.Set(f => f.Notebook, notebook?.Trim())
						.Set(f => f.Tags, _tags)
						.Inc(f => f.Version, 1)
						.Set(f => f.Modified, DateTime.UtcNow);

			Log.LogInformation($"Update note {note} for user {user}.");

			var _result = await Context.Note.UpdateOneAsync(_query, _set, new UpdateOptions { IsUpsert = true });

			return _result.IsAcknowledged && _result.ModifiedCount > 0;
		}

		/// <summary>
		/// Toggle trash status flag.
		/// </summary>
		/// <param name="note">The note id.</param>
		/// <param name="trash">Set trash flag for the note.</param>
		public async Task<bool> Trash(ObjectId note, bool trash)
		{
			var _filter = Builders<Note>.Filter;
			var _id = _filter.Eq(f => f.Id, note);

			var _query = _id;

			var _update = Builders<Note>.Update;
			var _set = _update
				.Set(f => f.Trash, trash);

			Log.LogInformation($"Mark note {note} as trash (Value: {trash}).");

			var _result = await Context.Note.UpdateOneAsync(_query, _set, new UpdateOptions { IsUpsert = true });

			return _result.IsAcknowledged && _result.ModifiedCount > 0;
		}

		/// <summary>
		/// Remove the note permanently.
		/// </summary>
		/// <param name="note">The note id.</param>
		public async Task<bool> Delete(ObjectId note)
		{
			var _filter = Builders<Note>.Filter;
			var _id = _filter.Eq(f => f.Id, note);

			var _query = _id;

			Log.LogInformation($"Delete note {note} permanently.");

			var _result = await Context.Note.DeleteOneAsync(_query);

			return _result.IsAcknowledged && _result.DeletedCount > 0;
		}

		/// <summary>
		/// Search for a term.
		/// </summary>
		/// <param name="term">The term to search for.</param>
		/// <param name="next">The next cursor.</param>
		/// <param name="limit">Limit the result.</param>
		/// <returns></returns>
		public async Task<IEnumerable<Note>> Search(string term, ObjectId next, int limit)
		{
			term = term?.Trim();

			if (String.IsNullOrEmpty(term))
			{
				return Enumerable.Empty<Note>();
			}

			var _filter = Builders<Note>.Filter;
			var _text = _filter.Text(term);
			var _active = _filter.Eq(f => f.Trash, false);

			var _projection = Builders<Note>.Projection;
			var _score = _projection.MetaTextScore("Score");

			var _sort = Builders<Note>.Sort;
			var _order = _sort.MetaTextScore("Score");

			var _query = _text & _active;

			Log.LogDebug($"Search notes for term '{term}'.");

			var _result = await Context.Note.Find(_query).Project<Note>(_score).Sort(_order).Limit(100).ToListAsync();
			if (next != ObjectId.Empty)
			{
				return _result.SkipWhile(condition => condition.Id != next).Skip(1).Take(limit);
			}

			return _result.Take(limit);
		}

		/// <summary>
		/// Get suggestions for tags.
		/// </summary>
		/// <param name="term">The term to search for.</param>
		/// <returns></returns>
		public async Task<IEnumerable<string>> TagSuggestions(string term)
		{
			term = term?.Trim();

			if (String.IsNullOrEmpty(term) || term.Length < 3)
				return Enumerable.Empty<string>();

			Log.LogDebug($"Get tag suggestions for term '{term}'.");

			var _result = await GetTags();

			return _result.Select(s => s.Id).Where(w => w.IndexOf(term, StringComparison.OrdinalIgnoreCase) != -1);
		}

		/// <summary>
		/// Get suggestions for notebooks.
		/// </summary>
		/// <param name="term">The term to search for.</param>
		/// <returns></returns>
		public async Task<IEnumerable<string>> NotebookSuggestions(string term)
		{
			term = term?.Trim();

			if (String.IsNullOrEmpty(term) || term.Length < 3)
				return Enumerable.Empty<string>();

			Log.LogDebug($"Get notebook suggestions for term '{term}'.");

			var _result = await GetNotebooks();

			return _result.Select(s => s.Id).Where(w => w.IndexOf(term, StringComparison.OrdinalIgnoreCase) != -1);
		}
	}
}