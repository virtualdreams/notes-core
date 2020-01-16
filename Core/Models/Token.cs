using System.ComponentModel.DataAnnotations.Schema;
using System;

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
		public int UserId { get; set; }

		public User User { get; set; }
	}
}