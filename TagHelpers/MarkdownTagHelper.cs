using Microsoft.AspNetCore.Razor.TagHelpers;
using notes.Extensions;

namespace notes.TagHelpers
{
	[HtmlTargetElement("markdown")]
	[HtmlTargetElement("p", Attributes = "markdown")]
	public class MarkdownTagHelper : TagHelper
	{
		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			if (output.TagName.Equals("markdown"))
				output.TagName = null;

			output.Attributes.RemoveAll("markdown");

			var _content = output.GetChildContentAsync().Result.GetContent();

			var _markdown = _content.ToMarkdown();

			output.Content.SetHtmlContent(_markdown ?? string.Empty);
		}
	}
}