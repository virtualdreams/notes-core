using System.Collections.Generic;

namespace notes.Areas.Admin.Models
{
	public class UserListContainer
	{
		public IEnumerable<UserModel> Users { get; set; }
	}
}