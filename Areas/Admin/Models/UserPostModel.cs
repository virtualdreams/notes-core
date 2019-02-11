using System.ComponentModel.DataAnnotations;

namespace notes.Areas.Admin.Models
{
	public class UserPostModel
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Please fill in a username.")]
		[MaxLength(100)]
		public string Username { get; set; }

		[MinLength(8, ErrorMessage = "Minimum length 8 characters.")]
		public string Password { get; set; }

		[MaxLength(50)]
		public string DisplayName { get; set; }

		[Required(ErrorMessage = "Please set a role.")]
		[RegularExpression("Administrator|User")]
		[MaxLength(50)]
		public string Role { get; set; }

		public bool Enabled { get; set; }
	}
}