using System;
using MongoDB.Bson;
using MongoDB.Driver;
using postit.Core.Helper;
using postit.Core.Models;

namespace postit.Core.Services
{
	public class UserService
	{
		private readonly MongoContext Context;

		public UserService(MongoContext context)
		{
			Context = context;
		}

		public User GetById(ObjectId id)
		{
			throw new NotImplementedException();
		}

		internal User GetByName(string username)
        {
            var _filter = Builders<User>.Filter;
			var _username = _filter.Eq(f => f.Username, username);
			var _active = _filter.Eq(f => f.Enabled, true);

			return Context.User.Find(_username & _active).SingleOrDefault();
        }

		public ObjectId Create(string username, string password, string role)
		{
			username = username.Trim();
			password = password.Trim();

			var _user = new User
			{
				Username = username,
				Password = PasswordHasher.HashPassword(password),
				Role = role.ToString(),
				Enabled = true
			};

			Context.User.InsertOne(_user);

			if(_user.Id == ObjectId.Empty)
			{
				return ObjectId.Empty;
			}
			else
			{
				return _user.Id;
			}
		}

        public User Login(string username, string password)
		{
			username = username.Trim();
			password = password.Trim();

			var _filter = Builders<User>.Filter;
			var _username = _filter.Eq(f => f.Username, username);
			var _active = _filter.Eq(f => f.Enabled, true);

			var _user = Context.User.Find(_username & _active).SingleOrDefault();
			if(_user != null && PasswordHasher.VerifyHashedPassword(_user.Password, password))
				return _user;
			
			return null;
		}
	}
}