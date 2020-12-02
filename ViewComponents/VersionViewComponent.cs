using Microsoft.AspNetCore.Mvc;
using notes.Core.Services;
using notes.Extensions;

namespace notes.ViewComponents
{
	public class VersionViewComponent : BaseViewComponent
	{
		private readonly IUserService UserService;

		public VersionViewComponent(IUserService user)
			: base(user)
		{
			UserService = user;
		}

		public IViewComponentResult Invoke() => Content($"{ApplicationVersion.InfoVersion()}");
	}
}
