using Microsoft.AspNetCore.Mvc;
using Notes.Core.Interfaces;

namespace Notes.ViewComponents
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
