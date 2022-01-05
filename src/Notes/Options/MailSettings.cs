namespace Notes.Options
{
	public class MailSettings
	{
		public const string MailSettingsName = "Mail";

		/// <summary>
		/// Enable sending emails.
		/// </summary>
		public bool Enabled { get; set; } = false;

		/// <summary>
		/// Smtp server.
		/// </summary>
		public string Host { get; set; } = "localhost";

		/// <summary>
		/// Smtp port.
		/// </summary>
		public int Port { get; set; } = 25;

		/// <summary>
		/// Smtp login username.
		/// </summary>
		public string Username { get; set; } = null;

		/// <summary>
		/// Smtp login password.
		/// </summary>
		public string Password { get; set; } = null;

		/// <summary>
		/// Smtp from mail.
		/// </summary>
		public string MailFrom { get; set; } = "admin@localhost";

		/// <summary>
		/// Skip verify SSL certificates.
		/// </summary>
		public bool DisableCertificateValidation { get; set; } = false;
	}
}