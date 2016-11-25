using CommonMark;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace postit
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

			var _markdown = CommonMarkConverter.Convert(_content);

			//var _repl = Regex.Replace(_c1, @"/(p|u)/([a-zA-Z0-9]+)", @"<a href=""/$1/$2"">/$1/$2</a>");
			//_repl = Regex.Replace(_repl, @"(((https?|ftp)://|www.)[^\s<]+[^\s<\.)])", @"<a href=""$1"">$1</a>");
			//_repl = _repl.Replace(@"\n", "<br />");

			output.Content.SetHtmlContent(_markdown ?? string.Empty);
		}
	}
}