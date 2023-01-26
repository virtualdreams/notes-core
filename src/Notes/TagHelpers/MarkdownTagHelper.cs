using Microsoft.AspNetCore.Razor.TagHelpers;
using Notes.Extensions;
using System;

namespace Notes.TagHelpers
{
	[HtmlTargetElement("markdown", Attributes = ContentAttributeName, TagStructure = TagStructure.WithoutEndTag)]
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