using System.ComponentModel.DataAnnotations;

namespace notes.Models
{
	public class PasswdResetPostModel
	{
		public string NewPassword { get; set; }

		public string ConfirmPassword { get; set; }
	}
}