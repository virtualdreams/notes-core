using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System;
using notes.Core.Data;
using notes.Core.Internal;
using notes.Core.Models;
using notes.Services;

namespace notes.Core.Services
{
	public class UserService : IUserService
	{
		private readonly ILogger<UserService> Log;
		private readonly DataContext Context;
		private readonly IMailService MailService;
		private readonly ITokenService TokenService;

		public UserService(ILogger<UserService> log, DataContext context, IMailService mail, ITokenService token)
		{
			Log = log;
			Context = context;
			MailService = mail;
			TokenService = token;
		}

		/// <summary>
		/// Get all available users.
		/// </summary>
		/// <returns>A list of users.</returns>
		public async Task<List<User>> GetUsersAsync()
		{
			Log.LogInformation("Get all users.");

			var _result = await Context.User
				.AsNoTracking()
				.OrderBy(o => o.Role)
				.ThenBy(o => o.Username)
				.ToListAsync();

			return _result;
		}

		/// <summary>
		/// Test, if the database has users.
		/// </summary>
		/// <returns>True if any user exists.</returns>
		public async Task<bool> HasUsersAsync()
		{
			var _result = await Context.User
				.AsNoTracking()
				.CountAsync();

			Log.LogDebug($"Database contains {_result} users.");

			return _result > 0;
		}

		/// <summary>
		/// Get count of available administrators.
		/// </summary>
		/// <returns>Count if active adminsitrators</returns>
		public async Task<long> GetAdminCountAsync()
		{
			var _result = await Context.User
				.AsNoTracking()
				.Where(f => f.Role == "Administrator" && f.Enabled == true)
				.CountAsync();

			Log.LogDebug($"Database contains {_result} admins.");

			return _result;
		}

		/// <summary>
		/// Check if the given user is an administrator.
		/// </summary>
		/// <param name="id">The user id.</param>
		/// <returns>True if the user is an administrator.</returns>
		public async Task<bool> IsAdminAsync(int id)
		{
			var _result = await Context.User
				.AsNoTracking()
				.Where(f => f.Id == id && f.Role == "Administrator")
				.CountAsync();

			Log.LogDebug($"User {id} is admin: {_result == 1}.");

			return _result > 0;
		}

		/// <summary>
		/// Get a user by user id.
		/// </summary>
		/// <param name="id">The user id.</param>
		/// <returns>The user if exists or null.</returns>
		public async Task<User> GetByIdAsync(int id)
		{
			var _result = await Context.User
				.Where(f => f.Id == id)
				.SingleOrDefaultAsync();

			return _result;
		}

		/// <summary>
		/// Get a user by username.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <returns>The user if exists or null.</returns>
		public async Task<User> GetByNameAsync(string username)
		{
			Log.LogDebug($"Get user by name: '{username}'.");

			var _result = await Context.User
				.Where(f => f.Username == username)
				.SingleOrDefaultAsync();

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
		/// <returns>The new user.</returns>
		public async Task<User> CreateAsync(string username, string password, string displayName, string role, bool active, int pageSize)
		{
			username = username?.Trim()?.ToLower();
			password = password?.Trim();
			displayName = displayName?.Trim();

			var _currentDate = DateTime.UtcNow;

			var _user = new User
			{
				Username = username?.Trim()?.ToLower(),
				Password = !String.IsNullOrEmpty(password) ? PasswordHasher.HashPassword(password) : PasswordHasher.HashPassword(GenerateRandomPassword()),
				DisplayName = displayName?.Trim(),
				Role = role,
				Enabled = active,
				Created = _currentDate,
				Modified = _currentDate,
				Version = 1,
				Items = pageSize
			};

			try
			{
				Context.Add(_user);
				await Context.SaveChangesAsync();
			}
			catch (DbUpdateException ex) when ((ex.InnerException as MySqlException)?.Number == 1062)
			{
				throw new NotesDuplicateUsernameException();
			}

			Log.LogInformation($"Create new user '{username}' with id {_user.Id}.");

			return _user;
		}

		/// <summary>
		/// Update a user.
		/// </summary>
		/// <param name="id">The user id.</param>
		/// <param name="username">The username.</param>
		/// <param name="password">The paramref name="password". Leave empty if you don't want to change.</param>
		/// <param name="role">The user role. Can be "User" or "Administrator".</param>
		/// <param name="active">User account active.</param>
		public async Task UpdateAsync(int id, string username, string password, string displayName, string role, bool active)
		{
			username = username?.Trim()?.ToLower();
			password = password?.Trim();
			displayName = displayName?.Trim();

			if (await IsAdminAsync(id) && await GetAdminCountAsync() < 2 && (!active || !role.Equals("Administrator")))
			{
				Log.LogWarning($"The user {id} is the last available administrator. This account can't changed.");
				throw new NotesModifyAdminException();
			}

			var _user = await Context.User
				.Where(f => f.Id == id)
				.SingleOrDefaultAsync();

			if (_user == null)
				throw new NotesUserNotFoundException();

			var _currentDate = DateTime.UtcNow;

			_user.Username = username;
			// set a new password if password not empty
			if (!String.IsNullOrEmpty(password))
				_user.Password = PasswordHasher.HashPassword(password);
			_user.DisplayName = displayName?.Trim();
			_user.Role = role;
			_user.Enabled = active;
			_user.Modified = _currentDate;
			_user.Version += 1;

			Log.LogInformation($"Update user data for user {id}.");

			try
			{
				await Context.SaveChangesAsync();
			}
			catch (DbUpdateException ex) when ((ex.InnerException as MySqlException)?.Number == 1062)
			{
				throw new NotesDuplicateUsernameException();
			}
		}

		/// <summary>
		/// Delete a user permanently.
		/// </summary>
		/// <param name="id">The user id.</param>
		public async Task DeleteAsync(int id)
		{
			if (await IsAdminAsync(id) && await GetAdminCountAsync() < 2)
			{
				Log.LogWarning($"The user {id} is the last available administrator. This account can't deleted.");
				throw new NotesDeleteAdminException();
			}

			var _user = await Context.User
				.Where(f => f.Id == id)
				.SingleOrDefaultAsync();

			if (_user == null)
				throw new NotesUserNotFoundException();

			Context.User.Remove(_user);

			Log.LogInformation($"Delete user '{(await GetByIdAsync(id)).Username}' ({id}) permanently.");

			await Context.SaveChangesAsync();
		}

		/// <summary>
		/// Set a new password for the given user.
		/// </summary>
		/// <param name="id">The user id.</param>
		/// <param name="password">The new password.</param>
		public async Task UpdatePasswordAsync(int id, string password)
		{
			password = password?.Trim();

			var _user = await Context.User
				.Where(f => f.Id == id)
				.SingleOrDefaultAsync();

			if (_user == null)
				throw new NotesUserNotFoundException();

			var _currentDate = DateTime.UtcNow;

			_user.Password = PasswordHasher.HashPassword(password);
			_user.Modified = _currentDate;
			_user.Version += 1;

			Log.LogInformation($"Update password for user '{_user.Username}'.");

			await Context.SaveChangesAsync();
		}

		/// <summary>
		/// Update user settings.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <param name="displayName">The display name.</param>
		/// <param name="pageSize">The page size.</param>
		public async Task UpdateSettingsAsync(int id, string displayName, int pageSize)
		{
			displayName = displayName?.Trim();

			var _user = await Context.User
				.Where(f => f.Id == id)
				.SingleOrDefaultAsync();

			if (_user == null)
				throw new NotesUserNotFoundException();

			_user.DisplayName = displayName?.Trim();
			_user.Items = pageSize;

			Log.LogInformation($"Update settings for user '{_user.Username}'.");

			await Context.SaveChangesAsync();
		}

		/// <summary>
		/// Get a user by username and password. If one of both false, null is returned. 
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="password">The paramref name="password".</param>
		/// <returns>The user if authenticated or null.</returns>
		public async Task<User> LoginAsync(string username, string password)
		{
			username = username?.Trim()?.ToLower();
			password = password?.Trim();

			var _user = await Context.User
				.Where(f => f.Username == username)
				.SingleOrDefaultAsync();

			if (_user != null && PasswordHasher.VerifyHashedPassword(_user.Password, password))
			{
				Log.LogInformation($"User '{_user.Username}' has been authenticated.");
				return _user;
			}

			Log.LogWarning($"User '{username}' failed to log in.");

			return null;
		}

		/// <summary>
		/// Create a reset token and send an email to the user.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="origin">The origin url.</param>
		/// <returns></returns>
		public async Task ForgotPasswordAsync(string username, string origin)
		{
			username = username?.Trim()?.ToLower();

			var _user = await Context.User
				.Where(f => f.Username == username)
				.SingleOrDefaultAsync();

			if (_user == null || !_user.Enabled)
			{
				Log.LogWarning($"User '{username}' does not exist. No mail sent.");
				return;
			}

			var _passwordResetToken = TokenService.GeneratePasswordResetToken(_user.Password, _user.Username, 60);

			Log.LogInformation($"Create password reset token for user '{_user.Username}'.");

			await MailService.SendResetPasswordMailAsync(!String.IsNullOrEmpty(_user.DisplayName) ? _user.DisplayName : _user.Username, _user.Username, origin, _passwordResetToken);
		}

		/// <summary>
		/// Get a user by reset token.
		/// </summary>
		/// <param name="token">The token.</param>
		/// <returns></returns>
		public async Task<User> GetByTokenAsync(string token)
		{
			try
			{
				var _username = TokenService.GetUsernameFromRefreshToken(token);

				Log.LogInformation($"Get user from from password reset token: '{_username}'.");

				var _user = await Context.User
					.Where(f => f.Username == _username)
					.SingleOrDefaultAsync();

				if (_user == null || !_user.Enabled)
				{
					Log.LogInformation($"User '{_username}' does not exist.");
					return null;
				}

				Log.LogInformation($"Validate password reset token for user '{_user.Username}'.");

				TokenService.GetPrincipalFromRefreshToken(token, _user.Password);
				// TODO check _username and _user.Username

				Log.LogInformation($"Validation of the password reset token for user '{_user.Username}' passed.");

				return _user;
			}
			catch (Exception ex)
			{
				Log.LogError($"Validate password reset token failed: '{ex.Message}'.");

				throw new NotesInvalidTokenException();
			}
		}

		/// <summary>
		/// Generate a random password.
		/// </summary>
		/// <returns>Random password.</returns>
		private string GenerateRandomPassword()
		{
			var _buffer = new byte[20];
			using (var rng = RandomNumberGenerator.Create())
			{
				rng.GetNonZeroBytes(_buffer);
			}

			return Convert.ToBase64String(_buffer);
		}
	}
}