using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using notes.Core.Models;

namespace notes.Core.Services
{
    public class NoteService
	{
		private readonly MongoContext Context;

		public NoteService(MongoContext context)
		{
			Context = context;
		}

		/// <summary>
		/// Get a list of notes.
		/// </summary>
		/// <param name="user">The user who owns the notes.</param>
		/// <param name="offset">The page offset.</param>
		/// <param name="limit">The page limit.</param>
		/// <returns></returns>
		public IEnumerable<Note> Get(ObjectId user, bool trashed, int? offset, int? limit)
		{
			var _filter = Builders<Note>.Filter;
			var _user = _filter.Eq(f => f.Owner, user);
			var _active = _filter.Eq(f => f.Trash, trashed);

			return Context.Note.Find(_user & _active).SortByDescending(f => f.Id).Skip(offset).Limit(limit).ToEnumerable();
		}

		public IEnumerable<Note> GetByNotebook(ObjectId user, string notebook, int? offset, int? limit)
		{
			notebook = notebook?.Trim();

			if(notebook == null)
				return Enumerable.Empty<Note>();

			var _filter = Builders<Note>.Filter;
			var _id = _filter.Eq(f => f.Notebook, notebook);
			var _user = _filter.Eq(f => f.Owner, user);
			var _trash = _filter.Eq(f => f.Trash, false);

			return Context.Note.Find(_id & _user & _trash).Skip(offset).Limit(limit).ToEnumerable();
		}

		public IEnumerable<Note> GetByTag(ObjectId user, string tag, int? offset, int? limit)
		{
			tag = tag?.Trim();

			if(tag == null)
				return Enumerable.Empty<Note>();

			var _filter = Builders<Note>.Filter;
			var _id = _filter.AnyIn("Tags", new string[] { tag });
			var _user = _filter.Eq(f => f.Owner, user);
			var _trash = _filter.Eq(f => f.Trash, false);

			return Context.Note.Find(_id & _user & _trash).Skip(offset).Limit(limit).ToEnumerable();
		}

		/// <summary>
		/// Get a note by id.
		/// </summary>
		/// <param name="note">The note id.</param>
		/// <param name="user">The user id.</param>
		/// <returns></returns>
		public Note GetById(ObjectId note, ObjectId user)
		{
			var _filter = Builders<Note>.Filter;
			var _id = _filter.Eq(f => f.Id, note);
			var _user = _filter.Eq(f => f.Owner, user);

			return Context.Note.Find(_id & _user).SingleOrDefault();
		}

		/// <summary>
		/// Get a list of notebooks.
		/// </summary>
		/// <param name="user">The users notebooks.</param>
		/// <returns></returns>
		public IEnumerable<string> Notebooks(ObjectId user)
		{
			return Context.Note.Distinct<string>("Notebook", new ExpressionFilterDefinition<Note>(f => f.Owner == user && f.Trash == false)).ToEnumerable().Where(s => !String.IsNullOrEmpty(s));
		}

		public IEnumerable<string> Tags(ObjectId user)
		{
			return Context.Note.Distinct<string>("Tags", new ExpressionFilterDefinition<Note>(f => f.Owner == user && f.Trash == false)).ToEnumerable().Where(s => !String.IsNullOrEmpty(s));
		}

		/// <summary>
		/// Set the notebook name for a note.
		/// </summary>
		/// <param name="note">The note.</param>
		/// <param name="user">The user who owns the note.</param>
		/// <param name="notebook">The notebook name.</param>
		public void SetNotebook(ObjectId note, string notebook)
		{
			notebook = notebook?.Trim();

			var _filter = Builders<Note>.Filter;
			var _id = _filter.Eq(f => f.Id, note);

			var _update = Builders<Note>.Update;
			var _set = _update
						.Set(f => f.Notebook, notebook);

			Context.Note.UpdateOne(_id, _set, new UpdateOptions { IsUpsert = true });
		}

		public void SetTags(ObjectId note, string tags)
		{
			var _tags = tags?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).Where(s => !String.IsNullOrEmpty(s)).ToArray();

			var _filter = Builders<Note>.Filter;
			var _id = _filter.Eq(f => f.Id, note);

			var _update = Builders<Note>.Update;
			var _set = _update
						.Set(f => f.Tags, _tags);

			Context.Note.UpdateOne(_id, _set, new UpdateOptions { IsUpsert = true });
		}

		/// <summary>
		/// Create a new note.
		/// </summary>
		/// <param name="user">The user who owns thos note.</param>
		/// <param name="title">The note title.</param>
		/// <param name="content">The note content.</param>
		/// <returns></returns>
		public ObjectId Create(ObjectId user, string title, string content)
		{
			var _insert = new Note
			{
				Owner = user,
				Title = title,
				Content = content,
				Tags = new string[] { },
				Notebook = String.Empty,
				Trash = false,
				Shared = false,
			};

			Context.Note.InsertOne(_insert);

			return _insert.Id;
		}

		public ObjectId Update(ObjectId note, string title, string content)
		{
			var _update = Builders<Note>.Update;
			var _set = _update
						.Set(f => f.Title, title)
						.Set(f => f.Content, content);

			Context.Note.UpdateOne(f => f.Id == note, _set, new UpdateOptions { IsUpsert = true });

			return note;
		}

		/// <summary>
		/// Toogle trash status flag.
		/// </summary>
		/// <param name="note"></param>
		/// <param name="user"></param>
		public void Trash(ObjectId note, bool trash)
		{
			// update filter
			var _filter = Builders<Note>.Filter;
			var _id = _filter.Eq(f => f.Id, note);

			// update values
			var _update = Builders<Note>.Update;
			var _set = _update
				.Set(f => f.Trash, trash);
			
			Context.Note.UpdateOne(_id, _set, new UpdateOptions { IsUpsert = true });
		}

		/// <summary>
		/// Remove the note.
		/// </summary>
		/// <param name="note">The note id.</param>
		/// <param name="user">The user who own this note.</param>
		public bool Delete(ObjectId note, ObjectId user)
		{
			var _filter = Builders<Note>.Filter;
			var _id = _filter.Eq(f => f.Id, note);
			var _user = _filter.Eq(f => f.Owner, user);

			Context.Note.DeleteOne(_id & _user);

			return GetById(note, user) == null;
		}

		public IEnumerable<Note> Search(ObjectId user, string term, bool trashed, int? offset, int? limit)
		{
			term = term?.Trim();

			var _filter = Builders<Note>.Filter;
			var _user = _filter.Eq(f => f.Owner, user);
			var _text = _filter.Text(term, "none");
			var _trash = _filter.Eq(f => f.Trash, trashed);

			return Context.Note.Find(_user & _text & _trash).Skip(offset).Limit(limit).ToEnumerable();
		}
	}
}