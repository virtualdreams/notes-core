using System.ComponentModel.DataAnnotations;

namespace notes.Models
{
	public class PasswdPostModel
	{
		[Required(ErrorMessage="Please fill in a password.")]
		[MinLength(8, ErrorMessage="Minimum length 8 characters.")]
		public string Password { get; set; }

		[Required(ErrorMessage="Please repeat the password.")]
		[MinLength(8, ErrorMessage="Minimum length 8 characters.")]
		public string PasswordRepeat { get; set; }
	}
}