using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;

namespace notes.Models
{
	public class UserPostModel
	{
		public ObjectId Id { get; set; }

		[Required(ErrorMessage="Please fill in a username.")]
		public string Username { get; set; }

		//[Required(ErrorMessage="Please fill in a password.")]
		[MinLength(8, ErrorMessage="Minimum length 8 characters.")]
		public string Password { get; set; }

		public string DisplayName { get; set; }

		[Required(ErrorMessage="Please set a role.")]
		[RegularExpression("Administrator|User")]
		public string Role { get; set; }

		public bool Enabled { get; set; }
	}
}