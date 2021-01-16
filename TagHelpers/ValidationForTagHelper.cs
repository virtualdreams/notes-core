using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Linq;
using System.Text.Encodings.Web;
using System;

namespace Notes.TagHelpers
{
	[HtmlTargetElement("input", Attributes = ValidationForAttributeName)]
	[HtmlTargetElement("textarea", Attributes = ValidationForAttributeName)]
	[HtmlTargetElement("select", Attributes = ValidationForAttributeName)]
	public class ValidationForTagHelper : TagHelper
	{
		private const string ValidationForAttributeName = "validation-for";

		[HtmlAttributeNotBound]
		[ViewContext]
		public ViewContext ViewContext { get; set; }

		[HtmlAttributeName(ValidationForAttributeName)]
		public string ValidateFor { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			var hasError = HasError(ValidateFor);

			if (hasError)
			{
				output.AddClass("is-invalid", HtmlEncoder.Default);
				output.PostElement.AppendHtml($@"<label id=""{ValidateFor}-error"" class=""error"" for=""{ValidateFor}"">{ErrorMessage(ValidateFor)}</label>");
			}
		}

		private bool HasError(string modelName)
		{
			if (ViewContext.ViewData.ModelState.ContainsKey(modelName))
			{
				return ViewContext.ViewData.ModelState[modelName].Errors.Count > 0;
			}

			return false;
		}

		private string ErrorMessage(string modelName)
		{
			if (ViewContext.ViewData.ModelState.ContainsKey(modelName))
			{
				return ViewContext.ViewData.ModelState[modelName].Errors.FirstOrDefault()?.ErrorMessage;
			}

			return String.Empty;
		}
	}
}