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
		/// Items per page.
		/// </summary>
		public int PageSize { get; set; }
	}
}