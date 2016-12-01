using MongoDB.Bson;

namespace notes.Models
{
	public class NotebookPostModel
	{
		public ObjectId Id { get; set; }

		public string Notebook { get; set; }
	}
}