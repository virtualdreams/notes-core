using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System;
using notes.Core.Helper;
using notes.Core.Models;

namespace notes.Core.Services
{
    public class UserService
	{
		private readonly ILogger<UserService> Log;
		private readonly MongoContext Context;
		private readonly IOptions<Settings> Options;

		public UserService(ILogger<UserService> log, MongoContext context, IOptions<Settings> options)
		{
			Log = log;
			Context = context;
			Options = options;
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

		/// <summary>
        /// Get user id.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
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

			Log.LogInformation("Update password for user {0}.", GetById(user).Username);

			Context.User.UpdateOne(_id & _active, _set);
		}

		/// <summary>
        /// Update user settings.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="pageSize">The new page size.</param>
        /// <param name="searchLanguage">The new search language.</param>
		public void SetSettings(ObjectId user, int pageSize, string searchLanguage)
		{
			var _filter = Builders<User>.Filter;
			var _id = _filter.Eq(f => f.Id, user);
			var _active = _filter.Eq(f => f.Enabled, true);

			var _update = Builders<User>.Update;
			var _set = _update
				.Set(f => f.Settings.PageSize, pageSize)
				.Set(f => f.Settings.SearchLanguage, searchLanguage);

			if(Log.IsEnabled(LogLevel.Debug))
			{
				Log.LogDebug("Update settings -> '{0}'", Context.User.UpdateOne(_id & _active, _set).ToString());
			}

			Context.User.UpdateOne(_id & _active, _set, new UpdateOptions { IsUpsert = true });
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

		/// <summary>
		/// Create a reset token and send an email to the user.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="origin">The origon url.</param>
		/// <returns></returns>
		public bool ForgotPassword(string username, string origin)
		{
			username = username?.Trim()?.ToLower();

			// get user from database
			var user = GetByName(username);
			if(user == null)
				return false;

			// create reset token
			var nonce = ObjectId.GenerateNewId().ToString();
			Context.Token.InsertOne(new Token {
				Created = DateTime.Now,
				User = user.Id,
				Nonce = nonce
			});

			// create reset password e-mail
			var message = new MimeMessage();
			message.From.Add(new MailboxAddress(Options.Value.Smtp.From));
			message.To.Add(new MailboxAddress(username));
			message.Subject = $"[{Options.Value.SiteName}] - Reset Password";
			message.Body = new TextPart("plain")
			{
				Text = 
$@"Hi {username},

You recently requested to reset your password for your {Options.Value.SiteName} account. Use the link below to reset it. This password reset is only valid for the next <TIME> hours.

{origin}/account/reset_password/{nonce}

If you did not request a password reset, please ignore this email or contact support if you have questions.

Thanks,
The {Options.Value.SiteName} Team
				
{Options.Value.SiteName} ({origin})"
			};

			// send e-mail
			using(var client = new SmtpClient()) {
				// accept all SSL certificates (in case the server supports STARTTLS)
				if(Options.Value.Smtp.SkipVerify)
                	client.ServerCertificateValidationCallback = (s,c,h,e) => true;
				
				client.Connect(Options.Value.Smtp.Server, Options.Value.Smtp.Port, false);
				
				// disable authentication if username or password missing
				if(!String.IsNullOrEmpty(Options.Value.Smtp.Username) && !String.IsNullOrEmpty(Options.Value.Smtp.Passwd))
					client.Authenticate(Options.Value.Smtp.Username, Options.Value.Smtp.Passwd);
				
				client.Send(message);
				client.Disconnect(true);
			}

			return true;
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

			return GetById(_token.User);
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