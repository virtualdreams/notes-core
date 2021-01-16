namespace Notes.Areas.Admin.Models
{
	public class UserPostModel
	{
		public int Id { get; set; }

		public string Username { get; set; }

		public string Password { get; set; }

		public string DisplayName { get; set; }

		public string Role { get; set; }

		public bool Enabled { get; set; }
	}
}