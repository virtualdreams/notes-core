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

		public IEnumerable<Postit> Get(int offset, int limit)
		{
			return Context.Postit.Find(_ => true).SortByDescending(f => f.Id).Skip(offset).Limit(limit).ToEnumerable();
		}

		public Postit GetById(ObjectId id)
		{
			return Context.Postit.Find(f => f.Id == id).SingleOrDefault();
		}

		public ObjectId Create(ObjectId user, string title, string content)
		{
			var _insert = new Postit
			{
				Title = title,
				Content = content
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

			Context.Postit.UpdateOne(f => f.Id == postit, _set);

			return postit;
		}
	}
}