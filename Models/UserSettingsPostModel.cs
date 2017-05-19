using System.ComponentModel.DataAnnotations;

namespace notes.Models
{
	public class UserSettingsPostModel
	{
		[Required]
		[Range(1, 100)]
		public int Items { get; set; }
	}
}