using System;

namespace Notes.Areas.Admin.Models
{
	public class UserModel
	{
		public int Id { get; set; }

		public string Username { get; set; }

		public string DisplayName { get; set; }

		public string Role { get; set; }

		public bool Enabled { get; set; }

		public DateTime Created { get; set; }

		public DateTime Modified { get; set; }

		public int Version { get; set; }
	}
}