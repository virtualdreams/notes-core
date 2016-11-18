using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace postit.Core.Services
{
	public class PostitService
	{
		private readonly MongoContext Database;

		public PostitService(MongoContext context)
		{
			Database = context;
		}

		public IEnumerable<Postit> Get()
		{
			return Database.Postit.Find(_ => true).ToEnumerable();
		}

		public Postit GetById(ObjectId id)
		{
			return Database.Postit.Find(f => f.Id == id).SingleOrDefault();
		}
	}
}