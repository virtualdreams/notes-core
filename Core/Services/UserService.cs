using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System;
using notes.Core.Models;
using notes.Encryption;
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
		public IEnumerable<User> GetUsers()
		{
			Log.LogInformation("Get all users.");

			var _result = Context.User.Find(_ => true).ToEnumerable().OrderBy(o => o.Role).ThenBy(o => o.Username);

			return _result;
		}

		/// <summary>
		/// Test, if the database has users.
		/// </summary>
		/// <returns>True if any user exists.</returns>
		public bool HasUsers()
		{
			var _result = Context.User.Find(_ => true).Count() != 0;

			Log.LogDebug($"Test if database contains a user (Value: {_result}).");

			return _result;
		}

		/// <summary>
		/// Get count of available administrators.
		/// </summary>
		/// <returns>Count if active adminsitrators</returns>
		public long GetAdminCount()
		{
			var _result = Context.User.Find(f => f.Role.Equals("Administrator") && f.Enabled == true).Count();

			Log.LogDebug($"Get count of admins in database (Value: {_result}).");

			return _result;
		}

		/// <summary>
		/// Check if the given user is an administrator.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns>True if the user is an administrator.</returns>
		public bool IsAdmin(ObjectId user)
		{
			var _result = Context.User.Find(f => f.Id == user && f.Role.Equals("Administrator")).Count() == 1;

			Log.LogDebug($"Test if user {user} is admin (Value: {_result}).");

			return _result;
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

			var _query = _id;

			Log.LogDebug($"Get user by id {user}.");

			var _result = Context.User.Find(_query).SingleOrDefault();

			return _result;
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

			var _query = _username;

			Log.LogDebug($"Get user by name '{username}'.");

			var _result = Context.User.Find(_query).SingleOrDefault();

			return _result;
		}

		/// <summary>
		/// Get a user by reset token.
		/// </summary>
		/// <param name="token">The token.</param>
		/// <returns></returns>
		public User GetByToken(string token)
		{
			var _hash = new ResetToken(token).PrivateKey();

			var _filter = Builders<Token>.Filter;
			var _nonce = _filter.Eq(f => f.Nonce, _hash);

			var _query = _nonce;

			Log.LogInformation($"Get user by token '{_hash}'.");

			var _token = Context.Token.Find(_query).SingleOrDefault();
			if (_token == null)
				return null;

			return GetById(_token.User);
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

			var _query = _username;

			Log.LogDebug($"Get user id by username '{username}'.");

			var _result = Context.User.Find(_query).Project(f => f.Id).SingleOrDefault();

			return _result;
		}

		/// <summary>
		/// Get user settings.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <returns></returns>
		public UserSettings GetUserSettings(ObjectId user)
		{
			var _filter = Builders<User>.Filter;
			var _id = _filter.Eq(f => f.Id, user);
			var _active = _filter.Eq(f => f.Enabled, true);

			var _query = _id & _active;

			Log.LogDebug($"Get user settings {user}.");

			var _result = Context.User.Find(_query).Project(f => f.Settings).SingleOrDefault();

			return _result;
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
				Password = !String.IsNullOrEmpty(password) ? PasswordHasher.HashPassword(password) : null,
				DisplayName = displayName,
				Role = role,
				Enabled = active,
				Created = DateTime.UtcNow
			};

			try
			{
				Context.User.InsertOne(_user);
			}
			catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
			{
				throw new NotesDuplicateUsernameException();
			}

			Log.LogInformation($"Create new user '{username}' with id {_user.Id}.");

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

			if (IsAdmin(user) && GetAdminCount() < 2 && (!active || !role.Equals("Administrator")))
			{
				Log.LogWarning($"The user {user} is the last available administrator. This account can't changed.");
				throw new NotesModifyAdminException();
			}

			var _filter = Builders<User>.Filter;
			var _id = _filter.Eq(f => f.Id, user);

			var _query = _id;

			var _update = Builders<User>.Update;
			var _set = _update
				.Set(f => f.Username, username)
				.Set(f => f.DisplayName, displayName)
				.Set(f => f.Role, role)
				.Set(f => f.Enabled, active);

			// add set a new password if password not empty
			if (!String.IsNullOrEmpty(password))
				_set = _set.Set(f => f.Password, PasswordHasher.HashPassword(password));

			//Log.LogDebug(_set.Render(Context.User.DocumentSerializer, Context.User.Settings.SerializerRegistry).ToString());

			Log.LogInformation($"Update user data for user {user}.");

			try
			{
				Context.User.UpdateOne(_query, _set, new UpdateOptions { IsUpsert = true });
			}
			catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
			{
				Log.LogWarning($"The username '{username}' is already taken.");
				throw new NotesDuplicateUsernameException();
			}
		}

		/// <summary>
		/// Delete a user permanently.
		/// </summary>
		/// <param name="user">The user id.</param>
		public void Delete(ObjectId user)
		{
			if (IsAdmin(user) && GetAdminCount() < 2)
			{
				Log.LogWarning($"The user {user} is the last available administrator. This account can't deleted.");
				throw new NotesDeleteAdminException();
			}

			Log.LogInformation($"Delete user '{GetById(user).Username}' ({user}) permanently.");

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

			if (!new PasswordPolicy { NonAlphaLength = 0, UpperCaseLength = 0 }.IsValid(password))
				throw new NotesWeakPasswordException();

			var _filter = Builders<User>.Filter;
			var _id = _filter.Eq(f => f.Id, user);
			var _active = _filter.Eq(f => f.Enabled, true);

			var _query = _id & _active;

			var _update = Builders<User>.Update;
			var _set = _update.Set(f => f.Password, PasswordHasher.HashPassword(password));

			Log.LogInformation($"Update password for user '{GetById(user).Username}'.");

			Context.User.UpdateOne(_query, _set);
		}

		/// <summary>
		/// Update user settings.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <param name="pageSize">The new page size.</param>
		public void UpdateSettings(ObjectId user, int pageSize)
		{
			var _filter = Builders<User>.Filter;
			var _id = _filter.Eq(f => f.Id, user);

			var _query = _id;

			var _update = Builders<User>.Update;
			var _set = _update
				.Set(f => f.Settings.PageSize, pageSize);

			Log.LogInformation($"Update settings for user {user}.");

			Context.User.UpdateOne(_query, _set, new UpdateOptions { IsUpsert = true });
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

			var _query = _id;

			var _update = Builders<User>.Update;
			var _set = _update
				.Set(f => f.DisplayName, displayName);

			Log.LogInformation($"Update profile for user {user}.");

			Context.User.UpdateOne(_query, _set, new UpdateOptions { IsUpsert = true });
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

			var _query = _username & _active;

			Log.LogInformation($"Try to login user '{username}'.");

			var _user = Context.User.Find(_query).SingleOrDefault();
			if (_user != null && PasswordHasher.VerifyHashedPassword(_user.Password, password))
			{
				Log.LogInformation($"User '{username}' has been authenticated.");
				return _user;
			}

			Log.LogWarning($"User '{username}' failed to log in.");

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
			var _user = GetByName(username);
			if (_user == null || !_user.Enabled)
				return;

			Log.LogInformation($"Create password reset token for user '{username}'.");

			// create reset token
			var _token = ResetToken.CreateNew();
			Context.Token.InsertOne(new Token
			{
				Created = DateTime.Now,
				User = _user.Id,
				Nonce = _token.PrivateKey() // save token as sha512
			});

			Mail.SendResetPasswordMail(!String.IsNullOrEmpty(_user.DisplayName) ? _user.DisplayName : _user.Username, _user.Username, origin, _token.PublicKey()); // send the non hashed token as email
		}

		/// <summary>
		/// Remove the reset token.
		/// </summary>
		/// <param name="token">The token.</param>
		public void RemoveToken(string token)
		{
			var _hash = new ResetToken(token).PrivateKey();

			var _filter = Builders<Token>.Filter;
			var _nonce = _filter.Eq(f => f.Nonce, _hash);

			var _query = _nonce;

			Log.LogInformation($"Delete reset token '{_hash}'.");

			Context.Token.DeleteOne(_query);
		}
	}
}