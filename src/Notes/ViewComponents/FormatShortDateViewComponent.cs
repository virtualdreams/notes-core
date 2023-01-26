using Microsoft.AspNetCore.Mvc;
using Notes.Core.Interfaces;
using System;

namespace Notes.ViewComponents
{
	public class FormatShortDateViewComponent : BaseViewComponent
	{
		private readonly IUserService UserService;

		public FormatShortDateViewComponent(
			IUserService user)
			: base(user)
		{
			UserService = user;
		}

		public IViewComponentResult Invoke(DateTime date)
		{
			var _dt = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:ddd, dd MMM yyyy}", date);

			return Content($"{_dt}");
		}
	}
}