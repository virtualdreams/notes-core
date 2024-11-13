using Microsoft.AspNetCore.Razor.TagHelpers;
using Notes.Extensions;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Notes.TagHelpers
{
	[HtmlTargetElement("markdown", Attributes = ContentAttributeName, TagStructure = TagStructure.WithoutEndTag)]
	public class MarkdownTagHelper : TagHelper
	{
		private const string ContentAttributeName = "content";

		private const string PlainAttributeName = "plain";

		[HtmlAttributeName(ContentAttributeName)]
		public string Content { get; set; }

		[HtmlAttributeName(PlainAttributeName)]
		public bool Plain { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = null; // remove tag

			var _out = String.Empty;
			if (Plain)
			{
				// render content to plaintext
				var _plain = Content?.ToMarkdownPlain();

				// get top 10 lines
				var _plainArray = _plain.Split('\n').Take(10);

				// add lines as long as they are shorter than 300 characters.
				var _array = new List<string>();
				foreach (var _plainLine in _plainArray)
				{
					if (_plainLine.Length <= 300)
					{
						_array.Add(_plainLine);
					}
					else
					{
						break;
					}
				}

				// join all line with a break
				_out = String.Join("<br />", _array);
			}
			else
			{
				_out = Content?.ToMarkdownHtml();
			}

			output.Content.SetHtmlContent(_out ?? String.Empty);
		}
	}

	[HtmlTargetElement("markdown-inline")]
	public class MarkdownInlineTagHelper : TagHelper
	{
		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = null; // remove tag

			var _content = output.GetChildContentAsync().Result.GetContent();

			var _markdown = _content.ToMarkdownHtml();

			output.Content.SetHtmlContent(_markdown ?? String.Empty);
		}
	}
}