using Microsoft.AspNetCore.Mvc;
using Notes.Core.Interfaces;
using System;

namespace Notes.ViewComponents
{
	public class FormatShortDateViewComponent : ViewComponent
	{
		public FormatShortDateViewComponent()
		{ }

		public IViewComponentResult Invoke(DateTime date)
		{
			var _dt = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:ddd, dd MMM yyyy}", date);

			return Content($"{_dt}");
		}
	}
}