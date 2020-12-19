using System.Collections.Generic;
using System.Threading.Tasks;
using notes.Core.Models;

namespace notes.Core.Interfaces
{
	public interface IUserService
	{
		Task<List<User>> GetUsersAsync();
		Task<bool> HasUsersAsync();
		Task<long> GetAdminCountAsync();
		Task<bool> IsAdminAsync(int id);
		Task<User> GetByIdAsync(int id);
		Task<User> GetByNameAsync(string username);
		Task<User> CreateAsync(string username, string password, string displayName, string role, bool active, int pageSize);
		Task UpdateAsync(int id, string username, string password, string displayName, string role, bool active);
		Task DeleteAsync(int id);
		Task UpdatePasswordAsync(int id, string password);
		Task UpdateSettingsAsync(int id, string displayName, int pageSize);
		Task<User> LoginAsync(string username, string password);
		Task ForgotPasswordAsync(string username, string origin);
		Task<User> GetByTokenAsync(string token);
	}
}