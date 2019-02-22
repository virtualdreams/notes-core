using System.ComponentModel.DataAnnotations;
using notes.Extensions;

namespace notes.Models
{
	public class NotePostModel
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(100, ErrorMessage = "Please enter no more than 100 characters.")]
		public string Title { get; set; }

		[Required]
		public string Content { get; set; }

		[MaxLength(50, ErrorMessage = "Please enter no more than 50 characters.")]
		public string Notebook { get; set; }

		[StringArrayItemMaxLength(50, ErrorMessage = "Tag too long.")]
		public string Tags { get; set; }
	}
}