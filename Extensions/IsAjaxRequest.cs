using Microsoft.AspNetCore.Http;
using System;

namespace notes.Extensions
{
	static public class HttpRequestExtensions
	{
		/// <summary>
		/// Determines whether the specified HTTP request is an AJAX request.
		/// </summary>
		/// <param name="request">The HTTP request.</param>
		/// <returns>True if the specified HTTP request is an AJAX request; otherwise, false.</returns>
		static public bool IsAjaxRequest(this HttpRequest request)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}

			if (request.Headers != null)
			{
				return request.Headers["X-Requested-With"].Equals("XMLHttpRequest");
			}

			return false;
		}
	}
}