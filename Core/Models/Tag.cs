using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace notes.Core.Models
{
	[Table("tag")]
	public class Tag
	{
		[Column("id")]
		public int Id { get; set; }

		[Column("name")]
		[MaxLength(50)]
		public string Name { get; set; }

		[Column("noteid")]
		public int NoteId { get; set; }

		public Note Note { get; set; }
	}
}