using System.ComponentModel.DataAnnotations;

namespace notes.Models
{
	public class PasswdForgotPostModel
	{
		[Required(ErrorMessage="Please fill in a username.")]
		[EmailAddress]
		public string Username { get; set; }
	}
}