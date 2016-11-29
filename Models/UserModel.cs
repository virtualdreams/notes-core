using MongoDB.Bson;

namespace postit.Models
{
	public class UserModel
	{
		public ObjectId Id { get; set; }

		public string Username { get; set; }

		public string Role { get; set; }

		public bool Enabled { get; set; }
	}
}