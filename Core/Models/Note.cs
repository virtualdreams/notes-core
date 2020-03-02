using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace notes.Core.Models
{
	[Table("note")]
	public class Note
	{
		[Column("id")]
		public int Id { get; set; }

		[Column("title")]
		[MaxLength(100)]
		public string Title { get; set; }

		[Column("content")]
		public string Content { get; set; }

		[Column("notebook")]
		[MaxLength(50)]
		public string Notebook { get; set; }

		[Column("tags")]
		public ICollection<Tag> Tags { get; set; }

		[Column("trash")]
		public bool Trash { get; set; }

		[Column("created")]
		public DateTime Created { get; set; }

		[Column("modified")]
		public DateTime Modified { get; set; }
	}
}
