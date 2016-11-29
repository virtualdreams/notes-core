using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using postit.Core.Models;

namespace postit.Core.Services
{
    public class PostitService
	{
		private readonly MongoContext Context;

		public PostitService(MongoContext context)
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
		public IEnumerable<Postit> Get(ObjectId user, bool trashed, int? offset, int? limit)
		{
			var _filter = Builders<Postit>.Filter;
			var _user = _filter.Eq(f => f.Owner, user);
			var _active = _filter.Eq(f => f.Trash, trashed);

			return Context.Postit.Find(_user & _active).SortByDescending(f => f.Id).Skip(offset).Limit(limit).ToEnumerable();
		}

		public IEnumerable<Postit> GetByNotebook(ObjectId user, string notebook, int? offset, int? limit)
		{
			notebook = notebook?.Trim();

			if(notebook == null)
				return Enumerable.Empty<Postit>();

			var _filter = Builders<Postit>.Filter;
			var _id = _filter.Eq(f => f.Notebook, notebook);
			var _user = _filter.Eq(f => f.Owner, user);
			var _trash = _filter.Eq(f => f.Trash, false);

			return Context.Postit.Find(_id & _user & _trash).Skip(offset).Limit(limit).ToEnumerable();
		}

		/// <summary>
		/// Get a note by id.
		/// </summary>
		/// <param name="postit">The note id.</param>
		/// <param name="user">The user id.</param>
		/// <returns></returns>
		public Postit GetById(ObjectId postit, ObjectId user)
		{
			var _filter = Builders<Postit>.Filter;
			var _id = _filter.Eq(f => f.Id, postit);
			var _user = _filter.Eq(f => f.Owner, user);

			return Context.Postit.Find(_id & _user).SingleOrDefault();
		}

		/// <summary>
		/// Get a list of notebooks.
		/// </summary>
		/// <param name="user">The users notebooks.</param>
		/// <returns></returns>
		public IEnumerable<string> Notebooks(ObjectId user)
		{
			return Context.Postit.Distinct<string>("Notebook", new ExpressionFilterDefinition<Postit>(f => f.Owner == user && f.Trash == false)).ToEnumerable().Where(s => !String.IsNullOrEmpty(s));
		}

		/// <summary>
		/// Set the notebook name for a note.
		/// </summary>
		/// <param name="postit">The note.</param>
		/// <param name="user">The user who owns the note.</param>
		/// <param name="notebook">The notebook name.</param>
		public void SetNotebook(ObjectId postit, string notebook)
		{
			notebook = notebook?.Trim();

			var _filter = Builders<Postit>.Filter;
			var _id = _filter.Eq(f => f.Id, postit);

			var _update = Builders<Postit>.Update;
			var _set = _update
						.Set(f => f.Notebook, notebook);

			Context.Postit.UpdateOne(_id, _set, new UpdateOptions { IsUpsert = true });
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
			var _insert = new Postit
			{
				Owner = user,
				Title = title,
				Content = content,
				Tags = new string[] { },
				Notebook = String.Empty,
				Trash = false,
				Shared = false,
			};

			Context.Postit.InsertOne(_insert);

			return _insert.Id;
		}

		public ObjectId Update(ObjectId postit, string title, string content)
		{
			var _update = Builders<Postit>.Update;
			var _set = _update
						.Set(f => f.Title, title)
						.Set(f => f.Content, content);

			Context.Postit.UpdateOne(f => f.Id == postit, _set, new UpdateOptions { IsUpsert = true });

			return postit;
		}

		/// <summary>
		/// Toogle trash status flag.
		/// </summary>
		/// <param name="postit"></param>
		/// <param name="user"></param>
		public void Trash(ObjectId postit, bool trash)
		{
			// update filter
			var _filter = Builders<Postit>.Filter;
			var _id = _filter.Eq(f => f.Id, postit);

			// update values
			var _update = Builders<Postit>.Update;
			var _set = _update
				.Set(f => f.Trash, trash);
			
			Context.Postit.UpdateOne(_id, _set, new UpdateOptions { IsUpsert = true });
		}

		/// <summary>
		/// Remove the note.
		/// </summary>
		/// <param name="postit">The note id.</param>
		/// <param name="user">The user who own this note.</param>
		public void Delete(ObjectId postit)
		{
			var _filter = Builders<Postit>.Filter;
			var _id = _filter.Eq(f => f.Id, postit);

			Context.Postit.DeleteOne(_id);
		}

		public IEnumerable<Postit> Search(ObjectId user, string term, bool trashed, int? offset, int? limit)
		{
			term = term?.Trim();

			var _filter = Builders<Postit>.Filter;
			var _user = _filter.Eq(f => f.Owner, user);
			var _text = _filter.Text(term, "none");
			var _trash = _filter.Eq(f => f.Trash, trashed);

			return Context.Postit.Find(_user & _text & _trash).Skip(offset).Limit(limit).ToEnumerable();
		}
	}
}