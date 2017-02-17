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

		static public int ToMinutes(this DateTime dt)
		{
			var _diff = DateTime.Now - dt;

			return (int)_diff.TotalMinutes;
		}

		static public string ToWords(this int minutes)
		{
			// stolen from reddit https://github.com/reddit/reddit/blob/bd922104b971a5c6794b199f364a06fdf61359a2/r2/r2/public/static/js/timetext.js
			var chunks = new Chunk[]
			{
				new Chunk{ Val = 60 * 24 * 365, S1 = "a year ago", S2 = "{0} years ago" },
				new Chunk{ Val = 60 * 24 * 30, S1 = "a month ago", S2 = "{0} months ago" },
				new Chunk{ Val = 60 * 24, S1 = "a day ago", S2 = "{0} days ago" },
				new Chunk{ Val = 60, S1 = "an hour ago", S2 = "{0} hours ago" },
				new Chunk{ Val = 1, S1 = "a minute ago", S2 = "{0} minutes ago" },
			};

			foreach(var chunk in chunks)
			{
				var count = (int)Math.Floor((double)minutes / (double)chunk.Val);

				if(count > 0)
				{
					if(count == 1)
						return String.Format(chunk.S1, count);
					else
						return String.Format(chunk.S2, count);
				}
			}
			return "just now";
		}

		static public string GetUserName(this ClaimsPrincipal principal)
		{
			if (principal == null)
				throw new ArgumentNullException(nameof(principal));

			return principal.FindFirst(ClaimTypes.Name)?.Value;
		}

		static public string GetUserRole(this ClaimsPrincipal principal)
		{
			if (principal == null)
				throw new ArgumentNullException(nameof(principal));

			return principal.FindFirst(ClaimTypes.Role)?.Value;
		}
	}

	class Chunk
	{
		public int Val;
		public string S1;
		public string S2;
	}
}