using System.Reflection;

namespace notes.Extensions
{
	/// <summary>
	/// Get assembly version informations.
	/// </summary>
	public static class ApplicationVersion
	{
		/// <summary
		/// Get file version.
		/// </summary>
		/// <returns>File version.</returns>
		public static string FileVersion() => $"{System.Reflection.Assembly.GetEntryAssembly().GetName().Version}";

		/// <summary>
		/// Get informational version.
		/// </summary>
		/// <returns>Informational version.</returns>
		public static string InfoVersion() => $"{System.Reflection.Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}";
	}
}