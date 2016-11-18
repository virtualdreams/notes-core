using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace postit.Core.Services
{
	public class CommentService
	{
		private readonly MongoContext Context;

		public CommentService(MongoContext context)
		{
			Context = context;
		}

		public IEnumerable<Comment> GetByPost(ObjectId id)
		{
			var _filter = Builders<Comment>.Filter;
			var _post = _filter.Eq(f => f.Post, id);

			var _result = Context.Comment.Find(_post).ToString();

			var _count = Context.Comment.Find(_post).Count();

			return Context.Comment.Find(_post).ToEnumerable();
		}
	}
}