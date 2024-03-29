using System.Text.RegularExpressions;
using System;

namespace Notes.Extensions
{
	public static class NoteExtensions
	{
		private class TimeChunk
		{
			public int Val;
			public string S1;
			public string S2;
		}

		/// <summary>
		/// Slugify the string.
		/// </summary>
		/// <param name="value">The string to slugify.</param>
		/// <param name="maxLength">Max length of text.</param>
		/// <returns>Slugified string.</returns>
		public static string ToSlug(this string value, int maxLength = 100)
		{
			if (String.IsNullOrEmpty(value))
				return String.Empty;

			// convert to lower case
			value = value.ToLowerInvariant();

			// remove all accents
			//var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(value);
			//value = Encoding.ASCII.GetString(bytes);

			// replace spaces
			value = Regex.Replace(value, @"\s", "-");

			// replace underline
			value = Regex.Replace(value, @"_", "-");

			// replace german umlauts
			value = value.Replace("ä", "ae").Replace("ö", "oe").Replace("ü", "ue").Replace("ß", "ss");

			// remove invalid chars
			value = Regex.Replace(value, @"[^a-z0-9\s-]", "-");

			// trim dashes from end
			value = value.Trim('-');

			// replace double occurences of '-'
			value = Regex.Replace(value, @"([-]){2,}", "$1");

			// max length of text
			return value.Substring(0, value.Length <= maxLength ? value.Length : maxLength);
		}

		/// <summary>
		/// Convert datetime to minutes.
		/// </summary>
		/// <param name="dt">The datetime.</param>
		/// <returns></returns>
		public static int ToMinutes(this DateTime dt)
		{
			var _diff = DateTime.Now - dt;

			return (int)_diff.TotalMinutes;
		}

		/// <summary>
		/// Convert minutes to words.
		/// </summary>
		/// <param name="minutes">The minutes.</param>
		/// <returns></returns>
		public static string ToWords(this int minutes)
		{
			// stolen from reddit https://github.com/reddit/reddit/blob/bd922104b971a5c6794b199f364a06fdf61359a2/r2/r2/public/static/js/timetext.js
			var chunks = new TimeChunk[]
			{
				new TimeChunk{ Val = 60 * 24 * 365, S1 = "a year ago", S2 = "{0} years ago" },
				new TimeChunk{ Val = 60 * 24 * 30, S1 = "a month ago", S2 = "{0} months ago" },
				new TimeChunk{ Val = 60 * 24, S1 = "a day ago", S2 = "{0} days ago" },
				new TimeChunk{ Val = 60, S1 = "an hour ago", S2 = "{0} hours ago" },
				new TimeChunk{ Val = 1, S1 = "a minute ago", S2 = "{0} minutes ago" },
			};

			foreach (var chunk in chunks)
			{
				var count = (int)Math.Floor((double)minutes / (double)chunk.Val);

				if (count > 0)
				{
					if (count == 1)
						return String.Format(chunk.S1, count);
					else
						return String.Format(chunk.S2, count);
				}
			}
			return "just now";
		}

		/// <summary>
		/// Convert datetime to words.
		/// </summary>
		/// <param name="dt">The datetime to convert.</param>
		/// <returns></returns>
		public static string ToAge(this DateTime dt)
		{
			return dt.ToMinutes().ToWords();
		}
	}
}