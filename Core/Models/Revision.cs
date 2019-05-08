using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace notes.Core.Models
{
	[Table("revision")]
	public class Revision
	{
		[Column("id")]
		public int Id { get; set; }

		[Column("dt")]
		public DateTime Dt { get; set; }

		// Backreference
		[Column("noteid")]
		public int NoteId { get; set; }

		[Column("title")]
		[MaxLength(100)]
		public string Title { get; set; }

		[Column("content")]
		public string Content { get; set; }

		[Column("notebook")]
		[MaxLength(50)]
		public string Notebook { get; set; }

		// [Column("tags")]
		// public ICollection<Tag> Tags { get; set; }

		[Column("trash")]
		public bool Trash { get; set; }

		[Column("created")]
		public DateTime? Created { get; set; }

		[Column("modified")]
		public DateTime? Modified { get; set; }

		// Backreference
		public Note Note { get; set; }
	}
}