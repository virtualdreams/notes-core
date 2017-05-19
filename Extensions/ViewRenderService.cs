using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace notes.Extensions
{
	public interface IViewRenderService
	{
		Task<string> RenderToStringAsync(string viewName, object model);
	}

	public class ViewRenderService : IViewRenderService
	{
		private readonly IRazorViewEngine RazorViewEngine;
		private readonly ITempDataProvider TempDataProvider;
		private readonly IServiceProvider ServiceProvider;

		public ViewRenderService(IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
		{
			RazorViewEngine = razorViewEngine;
			TempDataProvider = tempDataProvider;
			ServiceProvider = serviceProvider;
		}

		public async Task<string> RenderToStringAsync(string viewName, object model)
		{
			var httpContext = new DefaultHttpContext { RequestServices = ServiceProvider };
			var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

			using (var sw = new StringWriter())
			{
				var viewResult = RazorViewEngine.FindView(actionContext, viewName, false);

				if (viewResult.View == null)
				{
					throw new ArgumentNullException($"{viewName} does not match any available view");
				}

				var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
				{
					Model = model
				};

				var viewContext = new ViewContext(
					actionContext,
					viewResult.View,
					viewDictionary,
					new TempDataDictionary(actionContext.HttpContext, TempDataProvider),
					sw,
					new HtmlHelperOptions()
				);

				await viewResult.View.RenderAsync(viewContext);
				return sw.ToString();
			}
		}
	}
}