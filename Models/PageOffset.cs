namespace notes.Models
{
    public class PageOffset
	{
		/// <summary>
		/// Current offset from zero.
		/// </summary>
		public int Offset { get; private set; }

		/// <summary>
		/// Has a previous section?
		/// </summary>
		public bool HasPrevious { get; private set; }

		/// <summary>
		/// Has a next section?
		/// </summary>
		public bool HasNext { get; private set; }

		public int PreviousOffset { get; private set; }

		public int NextOffset { get; private set; }

		/// <summary>
		/// The section size (pagesize).
		/// </summary>
		public int PageSize { get; private set; }

		/// <summary>
		/// Total items.
		/// </summary>
		public int Items { get; private set; }

		/// <summary>
		/// Initialize page offset.
		/// </summary>
		/// <param name="pageOffset">Current offset from zero.</param>
		/// <param name="pageSize">The section size.</param>
		/// <param name="items">Total items.</param>
		public PageOffset(int pageOffset, int pageSize, int items)
		{
			Items = (items < 0) ? 0 : items;
			PageSize = (pageSize < 0) ? 0 : pageSize;
			Offset = (pageOffset < 0) ? 0 : pageOffset;
			HasPrevious = (Offset == 0) ? false : true;
			HasNext = (Offset + PageSize < Items) ? true : false;
			PreviousOffset = Offset - PageSize;
			NextOffset = Offset + PageSize;
		}
	}
}