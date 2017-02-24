namespace notes
{
	public class Settings
	{
		/// <summary>
		/// MongoDb connection string.
		/// </summary>
		public string MongoDB { get; set; } = "mongodb://127.0.0.1/";

		/// <summary>
		/// MongoDb database name.
		/// </summary>
		public string Database { get; set; } = "notes";

		/// <summary>
		/// Site name.
		/// </summary>
		/// <returns></returns>
		public string SiteName { get; set; } = "Notes!";

		/// <summary>
		/// Items per page.
		/// </summary>
		public int PageSize { get; set; } = 10;

		/// <summary>
		/// Smtp settings
		/// </summary>
		public Smtp Smtp { get; set; } = new Smtp();
	}

	public class Smtp
	{
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