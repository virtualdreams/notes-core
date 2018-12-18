using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
		public async Task<IEnumerable<User>> GetUsers()
		{
			Log.LogInformation("Get all users.");

			var _task = await Context.User.Find(_ => true).ToListAsync();
			var _result = _task.OrderBy(o => o.Role).ThenBy(o => o.Username);

			return _result;
		}

		/// <summary>
		/// Test, if the database has users.
		/// </summary>
		/// <returns>True if any user exists.</returns>
		public async Task<bool> HasUsers()
		{
			var _result = await Context.User.Find(_ => true).CountDocumentsAsync();

			Log.LogDebug($"Database contains {_result} users.");

			return _result != 0;
		}

		/// <summary>
		/// Get count of available administrators.
		/// </summary>
		/// <returns>Count if active adminsitrators</returns>
		public async Task<long> GetAdminCount()
		{
			var _result = await Context.User.Find(f => f.Role.Equals("Administrator") && f.Enabled == true).CountDocumentsAsync();

			Log.LogDebug($"Database contains {_result} admins.");

			return _result;
		}

		/// <summary>
		/// Check if the given user is an administrator.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns>True if the user is an administrator.</returns>
		public async Task<bool> IsAdmin(ObjectId user)
		{
			var _result = await Context.User.Find(f => f.Id == user && f.Role.Equals("Administrator")).CountDocumentsAsync();

			Log.LogDebug($"User {user} is admin: {_result == 1}.");

			return _result == 1;
		}

		/// <summary>
		/// Get a user by user id.
		/// </summary>
		/// <param name="user">The user id.</param>
		/// <returns>The user if exists or null.</returns>
		public async Task<User> GetById(ObjectId user)
		{
			var _filter = Builders<User>.Filter;
			var _id = _filter.Eq(f => f.Id, user);

			var _query = _id;

			Log.LogDebug($"Get user by id: {user}.");

			var _result = await Context.User.Find(_query).SingleOrDefaultAsync();

			return _result;
		}

		/// <summary>
		/// Get a user by username.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <returns>The user if exists or null.</returns>
		public async Task<User> GetByName(string username)
		{
			var _filter = Builders<User>.Filter;
			var _username = _filter.Eq(f => f.Username, username);

			var _query = _username;

			Log.LogDebug($"Get user by name: '{username}'.");

			var _result = await Context.User.Find(_query).SingleOrDefaultAsync();

			return _result;
		}

		/// <summary>
		/// Get a user by reset token.
		/// </summary>
		/// <param name="token">The token.</param>
		/// <returns></returns>
		public async Task<User> GetByToken(string token)
		{
			var _hash = new ResetToken(token).PrivateKey();

			var _filter = Builders<Token>.Filter;
			var _nonce = _filter.Eq(f => f.Nonce, _hash);

			var _query = _nonce;

			Log.LogInformation($"Get user by token: '{_hash}'.");

			var _token = await Context.Token.Find(_query).SingleOrDefaultAsync();
			if (_token == null)
				return null;

			return await GetById(_token.User);
		}

		/// <summary>
		/// Get user id.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <returns></returns>
		public async Task<ObjectId> GetUserId(string username)
		{
			var _filter = Builders<User>.Filter;
			var _username = _filter.Eq(f => f.Username, username);

			var _query = _username;

			Log.LogDebug($"Get user id by username: '{username}'.");

			var _result = await Context.User.Find(_query).Project(f => f.Id).SingleOrDefaultAsync();

			return _result;
		}

		/// <summary>
		/// Get user settings.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <returns></returns>
		public async Task<UserSettings> GetUserSettings(ObjectId user)
		{
			var _filter = Builders<User>.Filter;
			var _id = _filter.Eq(f => f.Id, user);
			var _active = _filter.Eq(f => f.Enabled, true);

			var _query = _id & _active;

			Log.LogDebug($"Get user settings {user}.");

			var _result = await Context.User.Find(_query).Project(f => f.Settings).SingleOrDefaultAsync();

			return _result;
		}

		/// <summary>
		/// Create a new user with username, password and role.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="password">The password.</param>
		/// <param name="role">The role. Can be "User" or "Administrator".</param>
		/// <param name="active">User account active.</param>
		/// <param name="pageSize">Default page size.</param>
		/// <returns>The new ObjectId.</returns>
		public async Task<ObjectId> Create(string username, string password, string displayName, string role, bool active, int pageSize)
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
				Created = DateTime.UtcNow,
				Settings = new UserSettings
				{
					PageSize = pageSize
				}
			};

			try
			{
				await Context.User.InsertOneAsync(_user);
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
		public async Task<bool> Update(ObjectId user, string username, string password, string displayName, string role, bool active)
		{
			username = username?.Trim()?.ToLower();
			password = password?.Trim();
			displayName = displayName?.Trim();

			if (await IsAdmin(user) && await GetAdminCount() < 2 && (!active || !role.Equals("Administrator")))
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
				var _result = await Context.User.UpdateOneAsync(_query, _set, new UpdateOptions { IsUpsert = true });
				return _result.IsAcknowledged && _result.ModifiedCount > 0;
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
		public async Task<bool> Delete(ObjectId user)
		{
			if (await IsAdmin(user) && await GetAdminCount() < 2)
			{
				Log.LogWarning($"The user {user} is the last available administrator. This account can't deleted.");
				throw new NotesDeleteAdminException();
			}

			Log.LogInformation($"Delete user '{(await GetById(user)).Username}' ({user}) permanently.");

			var _result = await Context.User.DeleteOneAsync(f => f.Id == user);

			return _result.IsAcknowledged && _result.DeletedCount > 0;
		}

		/// <summary>
		/// Set a new password for the given user.
		/// </summary>
		/// <param name="user">The user id.</param>
		/// <param name="password">The new password.</param>
		public async Task<bool> UpdatePassword(ObjectId user, string password)
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

			Log.LogInformation($"Update password for user '{(await GetById(user)).Username}'.");

			var _result = await Context.User.UpdateOneAsync(_query, _set);

			return _result.IsAcknowledged && _result.ModifiedCount > 0;
		}

		/// <summary>
		/// Update user settings.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <param name="displayName">The display name.</param>
		/// <param name="pageSize">The page size.</param>
		public async Task<bool> UpdateSettings(ObjectId user, string displayName, int pageSize)
		{
			displayName = displayName?.Trim();

			var _filter = Builders<User>.Filter;
			var _id = _filter.Eq(f => f.Id, user);

			var _query = _id;

			var _update = Builders<User>.Update;
			var _set = _update
				.Set(f => f.DisplayName, displayName)
				.Set(f => f.Settings.PageSize, pageSize);

			Log.LogInformation($"Update settings for user {user}.");

			var _result = await Context.User.UpdateOneAsync(_query, _set, new UpdateOptions { IsUpsert = true });

			return _result.IsAcknowledged && _result.ModifiedCount > 0;
		}

		/// <summary>
		/// Get a user by username and password. If one of both false, null is returned. 
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="password">The paramref name="password".</param>
		/// <returns>The user if authenticated or null.</returns>
		public async Task<User> Login(string username, string password)
		{
			username = username?.Trim()?.ToLower();
			password = password?.Trim();

			var _filter = Builders<User>.Filter;
			var _username = _filter.Eq(f => f.Username, username);
			var _active = _filter.Eq(f => f.Enabled, true);

			var _query = _username & _active;

			Log.LogInformation($"Try to login user '{username}'.");

			var _user = await Context.User.Find(_query).SingleOrDefaultAsync();
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
		public async Task ForgotPassword(string username, string origin)
		{
			username = username?.Trim()?.ToLower();

			// get user from database
			var _user = await GetByName(username);
			if (_user == null || !_user.Enabled)
				return;

			Log.LogInformation($"Create password reset token for user '{username}'.");

			// create reset token
			var _token = ResetToken.CreateNew();
			await Context.Token.InsertOneAsync(new Token
			{
				Created = DateTime.Now,
				User = _user.Id,
				Nonce = _token.PrivateKey() // save token as sha512
			});

			await Mail.SendResetPasswordMail(!String.IsNullOrEmpty(_user.DisplayName) ? _user.DisplayName : _user.Username, _user.Username, origin, _token.PublicKey()); // send the non hashed token as email
		}

		/// <summary>
		/// Remove the reset token.
		/// </summary>
		/// <param name="token">The token.</param>
		public async Task<bool> RemoveToken(string token)
		{
			var _hash = new ResetToken(token).PrivateKey();

			var _filter = Builders<Token>.Filter;
			var _nonce = _filter.Eq(f => f.Nonce, _hash);

			var _query = _nonce;

			Log.LogInformation($"Delete reset token '{_hash}'.");

			var _result = await Context.Token.DeleteOneAsync(_query);

			return _result.IsAcknowledged && _result.DeletedCount > 0;
		}
	}
}