using Markdig;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace notes
{
    [HtmlTargetElement("markdown")]
	[HtmlTargetElement("p", Attributes = "markdown")]
    public class MarkdownTagHelper : TagHelper
    {
		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			if(output.TagName.Equals("markdown"))
				output.TagName = null;

			output.Attributes.RemoveAll("markdown");

			var _content = output.GetChildContentAsync().Result.GetContent();

			var _pipeline = new MarkdownPipelineBuilder()
				.UseNoFollowLinks()
				.UseSoftlineBreakAsHardlineBreak()
				.UseAbbreviations()
				.UseEmphasisExtras()
				.Build();
			var _markdown = Markdown.ToHtml(_content, _pipeline);

			output.Content.SetHtmlContent(_markdown ?? string.Empty);
		}
	}
}