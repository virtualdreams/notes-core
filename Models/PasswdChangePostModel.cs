namespace notes.Models
{
	public class PasswdChangePostModel
	{
		public string OldPassword { get; set; }

		public string NewPassword { get; set; }

		public string ConfirmPassword { get; set; }
	}
}