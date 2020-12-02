using Microsoft.AspNetCore.Mvc;
using System;
using notes.Core.Services;

namespace notes.ViewComponents
{
	public class FormatDateViewComponent : BaseViewComponent
	{
		private readonly IUserService UserService;

		public FormatDateViewComponent(IUserService user)
			: base(user)
		{
			UserService = user;
		}

		public IViewComponentResult Invoke(DateTime date)
		{
			var _dt = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:ddd, dd MMM yyyy HH:mm:ss}", date);

			return Content($"{_dt}");
		}
	}
}