using System.ComponentModel.DataAnnotations;

namespace notes.Models
{
	public class LoginModel
	{
		[Required]
		public string Username { get; set; }

		[Required]
		public string Password { get; set; }

		public bool Remember { get; set; }
	}
}