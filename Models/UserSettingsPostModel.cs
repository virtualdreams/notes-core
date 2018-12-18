using System.ComponentModel.DataAnnotations;

namespace notes.Models
{
	public class UserSettingsPostModel
	{
		[MaxLength(50, ErrorMessage = "Maximum length 8 characters.")]
		public string DisplayName { get; set; }

		[Required(ErrorMessage = "Please fill in a number.")]
		[Range(1, 100)]
		public int Items { get; set; }
	}
}