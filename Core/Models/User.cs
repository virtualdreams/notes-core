using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace Notes.Core.Models
{
	[Table("user")]
	public class User
	{
		[Column("id")]
		public int Id { get; set; }

		/// <summary>
		/// The username/loginname
		/// </summary>
		[Column("username")]
		[MaxLength(100)]
		public string Username { get; set; }

		/// <summary>
		/// The encrypted password
		/// </summary>
		[Column("password")]
		[MaxLength(100)]
		public string Password { get; set; }

		[Column("displayname")]
		[MaxLength(50)]
		public string DisplayName { get; set; }

		/// <summary>
		/// The user role.
		/// </summary>
		[Column("role")]
		[MaxLength(50)]
		public string Role { get; set; }

		/// <summary>
		/// User is active
		/// </summary>
		[Column("enabled")]
		public bool Enabled { get; set; }

		/// <summary>
		/// Created
		/// </summary>
		[Column("created")]
		public DateTime Created { get; set; }

		/// <summary>
		/// Modified
		/// </summary>
		[Column("modified")]
		public DateTime Modified { get; set; }

		/// <summary>
		/// Serial
		/// </summary>
		[Column("version")]
		public int Version { get; set; }

		/// <summary>
		/// Page size for user.
		/// </summary>
		[Column("items")]
		public int Items { get; set; }
	}
}