using System.Collections.Generic;

namespace Notes.Areas.Admin.Models
{
	public class UserListContainer
	{
		public IEnumerable<UserModel> Users { get; set; }
	}
}