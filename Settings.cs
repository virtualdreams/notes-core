namespace notes
{
	public class Settings
	{
		/// <summary>
		/// MongoDb connection string.
		/// </summary>
		public string MongoDB { get; set; }

		/// <summary>
		/// MongoDb database name.
		/// </summary>
		public string Database { get; set; }

		/// <summary>
		/// Site name.
		/// </summary>
		/// <returns></returns>
		public string SiteName { get; set; }

		/// <summary>
		/// Items per page.
		/// </summary>
		public int PageSize { get; set; }

		public Smtp Smtp { get; set; }
	}

	public class Smtp
	{
		/// <summary>
		/// Smtp server.
		/// </summary>
		public string Server { get; set; }

		/// <summary>
		/// Smtp port.
		/// </summary>
		public int Port { get; set; }

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
		public string From { get; set; }

		/// <summary>
		/// Skip verify SSL certificates.
		/// </summary>
		/// <returns></returns>
		public bool SkipVerify { get; set; }
	}
}