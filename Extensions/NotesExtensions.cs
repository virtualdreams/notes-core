using Markdig;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System;

namespace notes.Extensions
{
	static public class NoteExtensions
	{
		/// <summary>
		/// Slugify the string.
		/// </summary>
		/// <param name="value">The string to slugify.</param>
		/// <param name="maxLength">Max length of text.</param>
		/// <returns>Slugified string.</returns>
		static public string ToSlug(this string value, int maxLength = 100)
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
		static public int ToMinutes(this DateTime dt)
		{
			var _diff = DateTime.Now - dt;

			return (int)_diff.TotalMinutes;
		}

		/// <summary>
		/// Convert minutes to words.
		/// </summary>
		/// <param name="minutes">The minutes.</param>
		/// <returns></returns>
		static public string ToWords(this int minutes)
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
		static public string ToAge(this DateTime dt)
		{
			return dt.ToMinutes().ToWords();
		}

		/// <summary>
		/// Extract username from principal.
		/// </summary>
		/// <param name="principal">The principal.</param>
		/// <returns>The username.</returns>
		static public string GetUserName(this ClaimsPrincipal principal)
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
		static public string GetUserRole(this ClaimsPrincipal principal)
		{
			if (principal == null)
				throw new ArgumentNullException(nameof(principal));

			return principal.FindFirst(ClaimTypes.Role)?.Value;
		}

		/// <summary>
		/// Render markdown to html5.
		/// </summary>
		/// <param name="source">The markdown source.</param>
		/// <returns>The render output.</returns>
		static public string ToMarkdown(this string source)
		{
			var _pipeline = new MarkdownPipelineBuilder()
				.UseNoFollowLinks()
				.UseAbbreviations()
				.UseEmphasisExtras()
				.UseTaskLists()
				.UseAutoLinks()
				.UseMediaLinks()
				.UseBootstrap()
				.UsePipeTables()
				.Build();
			var _markdown = Markdown.ToHtml(source, _pipeline);

			return _markdown;
		}
	}

	static public class HtmlHelperExtensions
	{
		static public bool HasError(this IHtmlHelper helper, string modelName)
		{
			if (helper.ViewData.ModelState.ContainsKey(modelName))
			{
				return helper.ViewData.ModelState[modelName].Errors.Count > 0;
			}

			return false;
		}

		static public string ErrorMessage(this IHtmlHelper helper, string modelName)
		{
			if (helper.ViewData.ModelState.ContainsKey(modelName))
			{
				return helper.ViewData.ModelState[modelName].Errors.FirstOrDefault()?.ErrorMessage;
			}

			return String.Empty;
		}
	}

	class TimeChunk
	{
		public int Val;
		public string S1;
		public string S2;
	}
}