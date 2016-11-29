using System;
using CommonMark;
using HeyRed.MarkdownSharp;
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

#if MARKDOWN
			var _markdown = new Markdown().Transform(_content);
#else
			var _markdown = CommonMarkConverter.Convert(_content);
#endif

			output.Content.SetHtmlContent(_markdown ?? string.Empty);
		}
	}
}