using System.ComponentModel.DataAnnotations;

namespace notes.Models
{
	public class PasswdChangePostModel
	{
		[Required(ErrorMessage = "Please fill in a password.")]
		public string OldPassword { get; set; }

		[Required(ErrorMessage = "Please fill in a password.")]
		[MinLength(8, ErrorMessage = "Minimum length 8 characters.")]
		public string NewPassword { get; set; }

		[Required(ErrorMessage = "Please confirm the password.")]
		[MinLength(8, ErrorMessage = "Minimum length 8 characters.")]
		public string ConfirmPassword { get; set; }
	}
}