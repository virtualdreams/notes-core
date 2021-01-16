using System.Security.Claims;
using System;

namespace Notes.Extensions
{
	public static class ClaimsPrincipalExtensions
	{
		/// <summary>
		/// Extract username from principal.
		/// </summary>
		/// <param name="principal">The principal.</param>
		/// <returns>The username.</returns>
		public static string GetUserName(this ClaimsPrincipal principal)
		{
			if (principal == null)
				throw new ArgumentNullException(nameof(principal));

			return principal.FindFirst(ClaimTypes.Name)?.Value;
		}

		/// <summary>
		/// Extract the role from principal.
		/// </summary>
		/// <param name="principal">The principal.</param>
		/// <returns>The role.</returns>
		public static string GetUserRole(this ClaimsPrincipal principal)
		{
			if (principal == null)
				throw new ArgumentNullException(nameof(principal));

			return principal.FindFirst(ClaimTypes.Role)?.Value;
		}
	}
}