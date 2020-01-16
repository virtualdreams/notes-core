using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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