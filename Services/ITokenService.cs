using System.Security.Claims;

public interface ITokenService
{
	string GeneratePasswordResetToken(string securityKey, string username, int expire);
	ClaimsPrincipal GetPrincipalFromRefreshToken(string token, string securityKey);
	string GetUsernameFromRefreshToken(string token);
}