using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using notes.Core.Helper;
using notes.Core.Models;
using Microsoft.Extensions.Logging;

namespace notes.Core.Services
{
    public class UserService
	{
		private readonly ILogger<UserService> Log;
		private readonly MongoContext Context;

		public UserService(ILogger<UserService> log, MongoContext context)
		{
			Log = log;
			Context = context;
		}

		/// <summary>
		/// Get all available users.
		/// </summary>
		/// <returns>A list of users.</returns>
		public IEnumerable<User> Get()
		{
			return Context.User.Find(_ => true).ToEnumerable().OrderBy(o => o.Role).ThenBy(o => o.Username);
		}

		/// <summary>
		/// Test, if the database has users.
		/// </summary>
		/// <returns>True if any user exists.</returns>
		public bool HasUsers()
		{
			return Context.User.Find(_ => true).Count() != 0;
		}

		/// <summary>
		/// Get a user by user id.
		/// </summary>
		/// <param name="user">The user id.</param>
		/// <returns>The user if exists or null.</returns>
		public User GetById(ObjectId user)
		{
			var _filter = Builders<User>.Filter;
			var _id = _filter.Eq(f => f.Id, user);

			if(Log.IsEnabled(LogLevel.Debug))
			{
				Log.LogDebug("Get user by id -> '{0}'", Context.User.Find(_id).ToString());
			}

			return Context.User.Find(_id).SingleOrDefault();
		}

		/// <summary>
		/// Get a user by username.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <returns>The user if exists or null.</returns>
		public User GetByName(string username)
        {
            var _filter = Builders<User>.Filter;
			var _username = _filter.Eq(f => f.Username, username);
			var _active = _filter.Eq(f => f.Enabled, true);

			if(Log.IsEnabled(LogLevel.Debug))
			{
				Log.LogDebug("Get user by name -> '{0}'", Context.User.Find(_username & _active).ToString());
			}

			return Context.User.Find(_username & _active).SingleOrDefault();
        }

		public ObjectId GetUserId(string username)
		{
			var _filter = Builders<User>.Filter;
			var _username = _filter.Eq(f => f.Username, username);
			var _active = _filter.Eq(f => f.Enabled, true);

			if(Log.IsEnabled(LogLevel.Debug))
			{
				Log.LogDebug("Get user id -> '{0}'", Context.User.Find(_username & _active).Project(f => f.Id).ToString());
			}

			return Context.User.Find(_username & _active).Project(f => f.Id).SingleOrDefault();
		}

		/// <summary>
		/// Create a new user with username, password and role.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="password">The paramref name="password".</param>
		/// <param name="role">The role. Can be "User" or "Administrator".</param>
		/// <returns>The new ObjectId.</returns>
		public ObjectId Create(string username, string password, string role)
		{
			username = username?.Trim();
			password = password?.Trim();

			var _user = new User
			{
				Username = username,
				Password = PasswordHasher.HashPassword(password),
				Role = role.ToString(),
				Enabled = true
			};

			Context.User.InsertOne(_user);

			Log.LogInformation("Create new user {0} with id {1}.", username, _user.Id);

			if(_user.Id == ObjectId.Empty)
			{
				return ObjectId.Empty;
			}
			else
			{
				return _user.Id;
			}
		}

		/// <summary>
		/// Set a new password for the given user.
		/// </summary>
		/// <param name="user">The user id.</param>
		/// <param name="password">The new password.</param>
		public void SetPassword(ObjectId user, string password)
		{
			password = password?.Trim();

			var _filter = Builders<User>.Filter;
			var _id = _filter.Eq(f => f.Id, user);
			var _active = _filter.Eq(f => f.Enabled, true);

			var _update = Builders<User>.Update;
			var _set = _update.Set(f => f.Password, PasswordHasher.HashPassword(password));

			if(Log.IsEnabled(LogLevel.Debug))
			{
				Log.LogDebug("Set new password -> '{0}'", Context.User.UpdateOne(_id & _active, _set).ToString());
			}

			Log.LogInformation("Update password for user {0}.", GetById(user).Username);

			Context.User.UpdateOne(_id & _active, _set);
		}

		/// <summary>
		/// Get a user by username and password. If one of both false, null is returned. 
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="password">The paramref name="password".</param>
		/// <returns>The user if authenticated or null.</returns>
        public User Login(string username, string password)
		{
			username = username?.Trim();
			password = password?.Trim();

			var _filter = Builders<User>.Filter;
			var _username = _filter.Eq(f => f.Username, username);
			var _active = _filter.Eq(f => f.Enabled, true);

			if(Log.IsEnabled(LogLevel.Debug))
			{
				Log.LogDebug("Login -> '{0}'", Context.User.Find(_username & _active).ToString());
			}
			
			var _user = Context.User.Find(_username & _active).SingleOrDefault();
			if(_user != null && PasswordHasher.VerifyHashedPassword(_user.Password, password))
			{
				Log.LogInformation("User {0} has been authenticated.", username);
				return _user;
			}
			
			Log.LogWarning("User {0} failed to log in with password {1}.", username, password);
			return null;
		}
	}
}