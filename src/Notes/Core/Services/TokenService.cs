using Microsoft.IdentityModel.Tokens;
using Notes.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System;

namespace Notes.Core.Services
{
	public class TokenService : ITokenService
	{
		public TokenService()
		{ }

		/// <summary>
		/// Generate a password reset token.
		/// </summary>
		/// <param name="securityKey">The security key.</param>
		/// <param name="username">The username.</param>
		/// <param name="expire">The expire time in minutes.</param>
		/// <returns>A password reset token.</returns>
		public string GeneratePasswordResetToken(string securityKey, string username, int expire)
		{
			var _tokenHandler = new JwtSecurityTokenHandler();
			var _key = Encoding.UTF8.GetBytes(securityKey);
			var _tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]{
					new Claim(ClaimTypes.Name, username, ClaimValueTypes.String),
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				}),
				Audience = "notes-password-reset",
				Issuer = "notes",
				Expires = DateTime.UtcNow.AddMinutes(expire),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256Signature)
			};

			var _token = _tokenHandler.CreateToken(_tokenDescriptor);

			return _tokenHandler.WriteToken(_token);
		}

		/// <summary>
		/// Get principal from password reset token.
		/// </summary>
		/// <param name="token">The JWT token.</param>
		/// <param name="securityKey">The security key.</param>
		/// <returns>Principals from token.</returns>
		public ClaimsPrincipal GetPrincipalFromRefreshToken(string token, string securityKey)
		{
			var _tokenHandler = new JwtSecurityTokenHandler();
			var _tokenValidationParameters = new TokenValidationParameters
			{
				ValidateAudience = true,
				ValidAudience = "notes-password-reset",
				ValidateIssuer = true,
				ValidIssuer = "notes",
				ValidateLifetime = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey)),
			};

			SecurityToken _securityToken;
			var _principal = _tokenHandler.ValidateToken(token, _tokenValidationParameters, out _securityToken);

			return _principal;
		}

		/// <summary>
		/// Get username from password reset token.
		/// </summary>
		/// <param name="token">The JWT token</param>
		/// <returns>Ther username.</returns>
		public string GetUsernameFromRefreshToken(string token)
		{
			var _tokenHandler = new JwtSecurityTokenHandler();
			var _token = _tokenHandler.ReadJwtToken(token);
			var _username = _token.Claims.First(claim => claim.Type == "name").Value;

			return _username;
		}
	}
}