using Markdig;

namespace Notes.Extensions
{
	public static class MarkdownExtensions
	{
		/// <summary>
		/// Render markdown to html5.
		/// </summary>
		/// <param name="source">The markdown source.</param>
		/// <returns>The render output.</returns>
		public static string ToMarkdown(this string source)
		{
			var _pipeline = new MarkdownPipelineBuilder()
				.UseReferralLinks("nofollow")
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
}