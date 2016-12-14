using MongoDB.Bson;

namespace notes.Models
{
	public class Pager
	{
		public bool HasNext { get; set; }
		public ObjectId NextPage { get; set; }

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
		public Pager(ObjectId next, bool hasNext)
		{
			HasNext = hasNext;

			NextPage = next;
		}
	}
}