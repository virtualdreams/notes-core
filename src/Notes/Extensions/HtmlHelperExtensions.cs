using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System;

namespace Notes.Extensions
{
	public static class HtmlHelperExtensions
	{
		public static bool HasError(this IHtmlHelper helper, string modelName)
		{
			if (helper.ViewData.ModelState.ContainsKey(modelName))
			{
				return helper.ViewData.ModelState[modelName].Errors.Count > 0;
			}

			return false;
		}

		public static string ErrorMessage(this IHtmlHelper helper, string modelName)
		{
			if (helper.ViewData.ModelState.ContainsKey(modelName))
			{
				return helper.ViewData.ModelState[modelName].Errors.FirstOrDefault()?.ErrorMessage;
			}

			return String.Empty;
		}
	}
}