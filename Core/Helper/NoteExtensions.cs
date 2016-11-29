using System;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace notes.Helper
{
	static public class NoteExtensions
	{
		/// <summary>
		/// Slugify the string.
		/// </summary>
		/// <param name="value">The string to slugify.</param>
		/// <returns>Slugified string.</returns>
		static public string ToSlug(this string value)
		{
			// convert to lower case
			value = value.ToLowerInvariant();

			// remove all accents
			//var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(value);
			//value = Encoding.ASCII.GetString(bytes);

			// replace spaces
			value = Regex.Replace(value, @"\s", "-", RegexOptions.Compiled);

			// remove invalid chars
			value = Regex.Replace(value, @"[^a-z0-9\s-_]", "", RegexOptions.Compiled);

			// trim dashes from end
			value = value.Trim('-', '_');

			// replace double occurences of - or _
			value = Regex.Replace(value, @"([-_]){2,}", "$1", RegexOptions.Compiled);

			return value;
		}

		static public int AgeInMinutes(this DateTime dt)
		{
			var _diff = DateTime.Now - dt;
			//var _minutes = (_diff.Days * 24 * 60) + ((int)_diff.TotalMinutes .TotalSeconds / 60);

			return (int)_diff.TotalMinutes;
		}

		static public string AgeInWords(this int minutes)
		{
			if(minutes < 60)
				return String.Format("{0} minutes ago", minutes);
			
			if((minutes / 60) < 24)
				return String.Format("{0} hours ago", (minutes / 60));
			
			if((minutes / 60) >= 24 && (minutes / 60) < 48)
				return "yesterday";
			
			return String.Format("{0} days ago", ((minutes / 60) / 24));
		}

		public static string GetUserName(this ClaimsPrincipal principal)
		{
			if (principal == null)
				throw new ArgumentNullException(nameof(principal));

			return principal.FindFirst(ClaimTypes.Name)?.Value;
		}
	}
}