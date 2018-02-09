using MongoDB.Bson;

namespace notes.Models
{
	public class UserSettingsModel
	{
		public int Items { get; set; }

		public string Frontpage { get; set; }
	}
}