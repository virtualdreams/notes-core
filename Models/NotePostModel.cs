namespace notes.Models
{
	public class NotePostModel
	{
		public int Id { get; set; }

		public string Title { get; set; }

		public string Content { get; set; }

		public string Notebook { get; set; }

		public string Tags { get; set; }
	}
}