using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System;
using notes.Core.Models;
using notes.Helper;

namespace notes.Core.Services
{
    public class UserService
	{
		private readonly ILogger<UserService> Log;
		private readonly MongoContext Context;
		private readonly MailService Mail;

		public UserService(ILogger<UserService> log, MongoContext context, MailService mail)
		{
			Log = log;
			Context = context;
			Mail = mail;
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
		/// Test, if more than one administrator exists.
		/// </summary>
		/// <returns>True if more than one administrator exists.</returns>
		public long GetAdminCount()
		{
			return Context.User.Find(f => f.Role.Equals("Administrator") && f.Enabled == true).Count();
		}

		/// <summary>
		/// Test, if the given user is an administrator.
		/// </summary>
		/// <param name="user">The user object.</param>
		/// <returns>True if the user is an administrator.</returns>
		public bool IsAdmin(ObjectId user)
		{
			return Context.User.Find(f => f.Id == user && f.Role.Equals("Administrator")).Count() == 1;
		}

		/// <summary>
		/// Get a user by user id.
		/// </summary>
		/// <param name="user">The user id.</param>
		/// <returns>The user if exists or null.</returns>
		public User GetUserById(ObjectId user)
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
		public User GetUserByName(string username)
        {
            var _filter = Builders<User>.Filter;
			var _username = _filter.Eq(f => f.Username, username);

			if(Log.IsEnabled(LogLevel.Debug))
			{
				Log.LogDebug("Get user by name -> '{0}'", Context.User.Find(_username).ToString());
			}

			return Context.User.Find(_username).SingleOrDefault();
        }

		/// <summary>
		/// Get a user by reset token.
		/// </summary>
		/// <param name="nonce">The token.</param>
		/// <returns></returns>
		public User GetUserByToken(string nonce)
		{
			var _filter = Builders<Token>.Filter;
			var _nonce = _filter.Eq(f => f.Nonce, nonce);

			if(Log.IsEnabled(LogLevel.Debug))
			{
				Log.LogDebug("Get user by token -> '{0}'", Context.Token.Find(_nonce).ToString());
			}

			var _token = Context.Token.Find(_nonce).SingleOrDefault();
			if(_token == null)
				return null;

			return GetUserById(_token.User);
		}

		/// <summary>
        /// Get user id.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
		public ObjectId GetUserId(string username)
		{
			var _filter = Builders<User>.Filter;
			var _username = _filter.Eq(f => f.Username, username);

			if(Log.IsEnabled(LogLevel.Debug))
			{
				Log.LogDebug("Get user id -> '{0}'", Context.User.Find(_username).Project(f => f.Id).ToString());
			}

			return Context.User.Find(_username).Project(f => f.Id).SingleOrDefault();
		}

		/// <summary>
        /// Get user settings.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
		public UserSettings GetUserSettings(string username)
		{
			var _filter = Builders<User>.Filter;
			var _username = _filter.Eq(f => f.Username, username);
			var _active = _filter.Eq(f => f.Enabled, true);

			if(Log.IsEnabled(LogLevel.Debug))
			{
				Log.LogDebug("Get user id -> '{0}'", Context.User.Find(_username & _active).Project(f => f.Settings).ToString());
			}

			return Context.User.Find(_username & _active).Project(f => f.Settings).SingleOrDefault();
		}

		/// <summary>
		/// Create a new user with username, password and role.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="password">The password.</param>
		/// <param name="role">The role. Can be "User" or "Administrator".</param>
		/// <param name="active">User account active.</param>
		/// <returns>The new ObjectId.</returns>
		public ObjectId Create(string username, string password, string displayName, string role, bool active)
		{
			username = username?.Trim()?.ToLower();
			password = password?.Trim();
			displayName = displayName?.Trim();

			var _user = new User
			{
				Username = username,
				Password = PasswordHasher.HashPassword(password),
				DisplayName = displayName,
				Role = role,
				Enabled = active
			};

			try
			{
				Context.User.InsertOne(_user);
			}
			catch(MongoWriteException ex) when(ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
			{
				throw new NotesDuplicateUsernameException();
			}

			Log.LogInformation("Create new user {0} with id {1}.", username, _user.Id);

			return _user.Id;
		}

		/// <summary>
		/// Update a user.
		/// </summary>
		/// <param name="user">The user id.</param>
		/// <param name="username">The username.</param>
		/// <param name="password">The paramref name="password". Leave empty if you don't want to change.</param>
		/// <param name="role">The user role. Can be "User" or "Administrator".</param>
		/// <param name="active">User account active.</param>
		public void Update(ObjectId user, string username, string password, string displayName, string role, bool active)
		{
			username = username?.Trim()?.ToLower();
			password = password?.Trim();
			displayName = displayName?.Trim();

			if(IsAdmin(user) && GetAdminCount() < 2 && (!active || !role.Equals("Administrator")))
				throw new NotesModifyAdminException();

			var _filter = Builders<User>.Filter;
			var _id = _filter.Eq(f => f.Id, user);

			var _update = Builders<User>.Update;
			var _set = _update
				.Set(f => f.Username, username)
				.Set(f => f.DisplayName, displayName)
				.Set(f => f.Role, role)
				.Set(f => f.Enabled, active);

			// set new password if password not empty
			if(!String.IsNullOrEmpty(password))
				_set = _set.Set(f => f.Password, PasswordHasher.HashPassword(password));

			if(Log.IsEnabled(LogLevel.Debug))
				Log.LogDebug(_set.Render(Context.User.DocumentSerializer, Context.User.Settings.SerializerRegistry).ToString());

			Log.LogInformation("Update user data for {0}", user.ToString());

			try
			{
				Context.User.UpdateOne(_id, _set, new UpdateOptions { IsUpsert = true });
			}
			catch(MongoWriteException ex) when(ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
			{
				throw new NotesDuplicateUsernameException();
			}
		}

		/// <summary>
		/// Delete a user permanently.
		/// </summary>
		/// <param name="user">The user id.</param>
		public void Delete(ObjectId user)
		{
			if(IsAdmin(user) && GetAdminCount() < 2)
				throw new NotesDeleteAdminException();

			Log.LogInformation("Delete user {0} permanently.", GetUserById(user).Username);

			Context.User.DeleteOne(f => f.Id == user);
			Context.Note.DeleteMany(f => f.Owner == user);
		}

		/// <summary>
		/// Set a new password for the given user.
		/// </summary>
		/// <param name="user">The user id.</param>
		/// <param name="password">The new password.</param>
		public void UpdatePassword(ObjectId user, string password)
		{
			password = password?.Trim();

			var _filter = Builders<User>.Filter;
			var _id = _filter.Eq(f => f.Id, user);
			var _active = _filter.Eq(f => f.Enabled, true);

			var _update = Builders<User>.Update;
			var _set = _update.Set(f => f.Password, PasswordHasher.HashPassword(password));

			Log.LogInformation("Update password for user {0}.", GetUserById(user).Username);

			Context.User.UpdateOne(_id & _active, _set);
		}

		/// <summary>
        /// Update user settings.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="pageSize">The new page size.</param>
        /// <param name="searchLanguage">The new search language.</param>
		public void UpdateSettings(ObjectId user, int pageSize, string searchLanguage)
		{
			var _filter = Builders<User>.Filter;
			var _id = _filter.Eq(f => f.Id, user);

			var _update = Builders<User>.Update;
			var _set = _update
				.Set(f => f.Settings.PageSize, pageSize)
				.Set(f => f.Settings.SearchLanguage, searchLanguage);

			if(Log.IsEnabled(LogLevel.Debug))
			{
				Log.LogDebug("Update settings -> '{0}'", Context.User.UpdateOne(_id, _set).ToString());
			}

			Context.User.UpdateOne(_id, _set, new UpdateOptions { IsUpsert = true });
		}

		/// <summary>
		/// Update user profile.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <param name="displayName">The new display name.</param>
		public void UpdateProfile(ObjectId user, string displayName)
		{
			displayName = displayName?.Trim();

			var _filter = Builders<User>.Filter;
			var _id = _filter.Eq(f => f.Id, user);

			var _update = Builders<User>.Update;
			var _set = _update
				.Set(f => f.DisplayName, displayName);

			if(Log.IsEnabled(LogLevel.Debug))
			{
				Log.LogDebug("Update profile -> '{0}'", Context.User.UpdateOne(_id, _set).ToString());
			}

			Context.User.UpdateOne(_id, _set, new UpdateOptions { IsUpsert = true });
		}

		/// <summary>
		/// Get a user by username and password. If one of both false, null is returned. 
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="password">The paramref name="password".</param>
		/// <returns>The user if authenticated or null.</returns>
        public User Login(string username, string password)
		{
			username = username?.Trim()?.ToLower();
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
			
			Log.LogWarning("User {0} failed to log in.", username);
			return null;
		}

		/// <summary>
		/// Create a reset token and send an email to the user.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="origin">The origon url.</param>
		/// <returns></returns>
		public void ForgotPassword(string username, string origin)
		{
			username = username?.Trim()?.ToLower();

			// get user from database
			var _user = GetUserByName(username);
			if(_user == null || !_user.Enabled)
				return;

			// create reset token
			var nonce = ObjectId.GenerateNewId().ToString();
			Context.Token.InsertOne(new Token {
				Created = DateTime.Now,
				User = _user.Id,
				Nonce = nonce
			});

			Mail.SendResetPasswordMail(username, origin, nonce);
		}

		/// <summary>
		/// Remove the reset token.
		/// </summary>
		/// <param name="nonce">The token.</param>
		public void RemoveToken(string nonce)
		{
			var _filter = Builders<Token>.Filter;
			var _nonce = _filter.Eq(f => f.Nonce, nonce);

			if(Log.IsEnabled(LogLevel.Debug))
			{
				Log.LogDebug("Delete reset token -> '{0}'", Context.Token.DeleteOne(_nonce).ToString());
			}

			Context.Token.DeleteOne(_nonce);
		}
	}
}