using System.Threading.Tasks;

public interface IMailService
{
	Task SendResetPasswordMailAsync(string username, string mail, string origin, string token);
}