using System.Security.Claims;

namespace notes.Core.Interfaces
{
	public interface ITokenService
	{
		string GeneratePasswordResetToken(string securityKey, string username, int expire);
		ClaimsPrincipal GetPrincipalFromRefreshToken(string token, string securityKey);
		string GetUsernameFromRefreshToken(string token);
	}
}