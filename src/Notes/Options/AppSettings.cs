//using System.ComponentModel.DataAnnotations;

namespace Notes.Options
{
	public class AppSettings
	{
		public const string AppSettingsName = "Settings";

		/// <summary>
		/// Path to keystore directory.
		/// </summary>
		public string KeyStore { get; set; } = "";

		/// <summary>
		/// Site name.
		/// </summary>
		/// <returns></returns>
		public string SiteName { get; set; } = "Notes!";

		/// <summary>
		/// Items per page.
		/// </summary>
		public int PageSize { get; set; } = 10;
	}
}