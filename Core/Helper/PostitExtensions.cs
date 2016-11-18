using System;
using System.Text.RegularExpressions;

namespace postit.Helper
{
	static public class PostitExtensions
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
			var _minutes = (_diff.Days * 24 * 60) + (_diff.Seconds / 60);

			return _minutes;
		}

		static public string AgeInWords(this int minutes)
		{
			if(minutes < 60)
				return String.Format("vor {0} Minuten", minutes);
			
			if((minutes / 60) < 24)
				return String.Format("vor {0} Std.", (minutes / 60));
			
			if((minutes / 60) >= 24 && (minutes / 60) < 48)
				return "gestern";
			
			return String.Format("vor {0} Tagen", ((minutes / 60) / 24));
		}
	}
}