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

	public class NotesPasswordIncorrectException : NotesException
	{
		public NotesPasswordIncorrectException()
			: base("Old password isn't valid!")
		{ }
	}

	public class NotesPasswordMismatchException : NotesException
	{
		public NotesPasswordMismatchException()
			: base("Password doesn't match the confirmation!")
		{ }
	}

	public class NotesLoginFailedException : NotesException
	{
		public NotesLoginFailedException()
			: base("Incorrect username or password!")
		{ }
	}
}