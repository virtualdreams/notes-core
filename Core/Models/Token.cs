using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace notes.Core.Models
{
	[Table("token")]
	public class Token
	{
		[Column("id")]
		public int Id { get; set; }

		[Column("created")]
		public DateTime Created { get; set; }

		[Column("nonce")]
		public string Nonce { get; set; }

		[Column("userid")]
		public User User { get; set; }
	}
}