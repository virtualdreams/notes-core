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
		[BsonElement("username")]
		public string Username { get; set; }

		/// <summary>
		/// The encrypted password
		/// </summary>
		[BsonElement("password")]
		public string Password { get; set; }

		/// <summary>
		/// The user role.
		/// </summary>
		[BsonElement("role")]
		public string Role { get; set; }

		/// <summary>
		/// User is active
		/// </summary>
		[BsonElement("enabled")]
		public bool Enabled { get; set; }

		[BsonElement("settings")]
		public UserSettings Settings { get; set; }
	}

	public class UserSettings
	{
		[BsonElement("pagesize")]
		public int PageSize { get; set; }

		[BsonElement("searchlanguage")]
		public string SearchLanguage { get; set; }
	}
}