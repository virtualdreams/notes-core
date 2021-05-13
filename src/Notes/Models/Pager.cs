namespace Notes.Models
{
	public class Pager
	{
		public bool HasNext { get; set; }
		public int NextPage { get; set; }

		/// <summary>
		/// Initialize pager.
		/// </summary>
		public Pager()
		{ }

		/// <summary>
		/// Initialize pager.
		/// </summary>
		/// <param name="next">Next page id.</param>
		/// <param name="previous">Previous page id.</param>
		/// <param name="pageSize">Items per page.</param>
		public Pager(int next, bool hasNext)
		{
			HasNext = hasNext;

			NextPage = next;
		}
	}
}