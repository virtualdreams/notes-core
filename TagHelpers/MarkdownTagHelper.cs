using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using notes.Extensions;

namespace notes.TagHelpers
{
	[HtmlTargetElement("markdown", Attributes = ContentAttributeName)]
	public class MarkdownTagHelper : TagHelper
	{
		private const string ContentAttributeName = "content";

		[HtmlAttributeName(ContentAttributeName)]
		public string Content { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = null; // remove tag

			var _markdown = Content?.ToMarkdown();

			output.Content.SetHtmlContent(_markdown ?? String.Empty);
		}
	}

	[HtmlTargetElement("markdown-inline")]
	public class MarkdownInlineTagHelper : TagHelper
	{
		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = null; // remove tag

			var _content = output.GetChildContentAsync().Result.GetContent();

			var _markdown = _content.ToMarkdown();

			output.Content.SetHtmlContent(_markdown ?? String.Empty);
		}
	}
}