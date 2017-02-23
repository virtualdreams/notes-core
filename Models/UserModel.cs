using MongoDB.Bson;

namespace notes.Models
{
	public class UserModel
	{
		public ObjectId Id { get; set; }

		public string Username { get; set; }

		public string DisplayName { get; set; }

		public string Role { get; set; }

		public bool Enabled { get; set; }

		public string Created { get; set; }
	}
}