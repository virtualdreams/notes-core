using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Notes.Filter
{
	public class ModelStateDebugFilter : IAsyncActionFilter
	{
		private readonly ILogger<ModelStateDebugFilter> Logger;

		public ModelStateDebugFilter(
			ILogger<ModelStateDebugFilter> logger)
		{
			Logger = logger;
		}

		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			if (!context.ModelState.IsValid)
			{
				var _errorsInModelState = context.ModelState
					.Where(w => w.Value.Errors.Count > 0)
					.ToDictionary(d => d.Key, d => d.Value.Errors.Select(s => s.ErrorMessage)).ToArray();

				Logger.LogDebug("===== ModelState =====");
				Logger.LogDebug($"Path: {context.HttpContext.Request.Path}");
				foreach (var key in _errorsInModelState)
				{
					Logger.LogDebug($"Key: {key.Key}");
					foreach (var value in key.Value)
					{
						Logger.LogDebug($"     {value}");
					}
				}
				Logger.LogDebug("======================");
			}

			await next();
		}
	}
}