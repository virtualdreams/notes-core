using System.ComponentModel.DataAnnotations;

namespace notes.Models
{
	public class LoginModel
	{
		[Required(ErrorMessage = "Please fill in a username.")]
		public string Username { get; set; }

		[Required(ErrorMessage = "Please fill in a password.")]
		public string Password { get; set; }

		public bool Remember { get; set; }
	}
}