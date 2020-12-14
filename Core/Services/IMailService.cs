using System.Threading.Tasks;

namespace notes.Core.Services
{
	public interface IMailService
	{
		Task SendResetPasswordMailAsync(string username, string mail, string origin, string token);
	}
}