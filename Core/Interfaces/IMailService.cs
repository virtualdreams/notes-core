using System.Threading.Tasks;

namespace notes.Core.Interfaces
{
	public interface IMailService
	{
		Task SendResetPasswordMailAsync(string username, string mail, string origin, string token);
	}
}