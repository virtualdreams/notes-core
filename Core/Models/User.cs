using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace notes.Core.Models
{
	[BsonIgnoreExtraElements]
	public class User
	{
		[BsonId]
		[BsonIgnoreIfDefault]
		public ObjectId Id { get; set; }

		/// <summary>
		/// The username/loginname
		/// </summary>
		public string Username { get; set; }

		/// <summary>
		/// The encrypted password
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		/// The user role.
		/// </summary>
		public string Role { get; set; }

		/// <summary>
		/// User is active
		/// </summary>
		public bool Enabled { get; set; }
	}
}