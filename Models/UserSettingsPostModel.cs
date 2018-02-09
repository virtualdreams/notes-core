using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace notes.Models
{
	public class UserSettingsPostModel
	{
		[Required]
		[Range(1, 100)]
		public int Items { get; set; }

		public string Frontpage { get; set; }
	}
}