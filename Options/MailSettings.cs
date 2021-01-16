namespace Notes.Options
{
	public class MailSettings
	{
		public const string MailSettingsName = "Mail";

		/// <summary>
		/// Enable sending emails.
		/// </summary>
		public bool Enabled { get; set; }

		/// <summary>
		/// Smtp server.
		/// </summary>
		public string Server { get; set; } = "localhost";

		/// <summary>
		/// Smtp port.
		/// </summary>
		public int Port { get; set; } = 25;

		/// <summary>
		/// Smtp login username.
		/// </summary>
		public string Username { get; set; }

		/// <summary>
		/// Smtp login password.
		/// </summary>
		public string Passwd { get; set; }

		/// <summary>
		/// Smtp from mail.
		/// </summary>
		public string From { get; set; } = "admin@localhost";

		/// <summary>
		/// Skip verify SSL certificates.
		/// </summary>
		public bool SkipVerify { get; set; }
	}
}