using System;
using System.Collections.Generic;
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

		public IEnumerable<Postit> Get(ObjectId user, int offset, int limit)
		{
			var _filter = Builders<Postit>.Filter;
			var _user = _filter.Eq(f => f.Owner, user);
			var _active = _filter.Eq(f => f.Trash, false);

			return Context.Postit.Find(_user & _active).SortByDescending(f => f.Id).Skip(offset).Limit(limit).ToEnumerable();
		}

		public Postit GetById(ObjectId postit, ObjectId user)
		{
			var _filter = Builders<Postit>.Filter;
			var _id = _filter.Eq(f => f.Id, postit);
			var _user = _filter.Eq(f => f.Owner, user);

			return Context.Postit.Find(_id & _user).SingleOrDefault();
		}

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

		public void Trash(ObjectId postit, ObjectId user)
		{
			var _update = Builders<Postit>.Update;
			var _set = _update
				.Set(f => f.Trash, true);

			Context.Postit.UpdateOne(f => f.Id == postit, _set, new UpdateOptions { IsUpsert = true });
		}

		public void Restore(ObjectId postit, ObjectId user)
		{
			var _update = Builders<Postit>.Update;
			var _set = _update
				.Set(f => f.Trash, false);

			Context.Postit.UpdateOne(f => f.Id == postit, _set, new UpdateOptions { IsUpsert = true });
		}

		public void Delete(ObjectId postit, ObjectId user)
		{
			var _filter = Builders<Postit>.Filter;
			var _id = _filter.Eq(f => f.Id, postit);
			var _user = _filter.Eq(f => f.Owner, user);
			var _trash = _filter.Eq(f => f.Trash, false);

			Context.Postit.DeleteOne(_id & _user & _trash);
		}
	}
}