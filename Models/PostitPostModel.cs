using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;

namespace postit.Models
{
	public class PostitPostModel
	{
		
		public ObjectId Id { get; set; }

		[Required]
		public string Title { get; set; }

		public string Content { get; set; }
	}
}