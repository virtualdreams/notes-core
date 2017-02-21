using System;

namespace notes
{
	public class NotesException : Exception
	{
		public NotesException(string message)
			: base(message)
		{ }
	}

	public class DuplicateUsernameNotesException : NotesException
	{
		public DuplicateUsernameNotesException()
			: base("The username is already taken!")
		{ }
	}

	public class DeleteAdminNotesException : NotesException
	{
		public DeleteAdminNotesException()
			: base("The last administrator can't be deleted!")
		{ }
	}

	public class ModifyAdminNotesException : NotesException
	{
		public ModifyAdminNotesException()
			: base("The last administrator's role or state can't be changed!")
		{ }
	}
}