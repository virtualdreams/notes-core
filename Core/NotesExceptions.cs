using System;

namespace notes.Core
{
	public class NotesException : Exception
	{
		public NotesException(string message)
			: base(message)
		{ }
	}

	public class NotesDuplicateUsernameException : NotesException
	{
		public NotesDuplicateUsernameException()
			: base("The username is already taken!")
		{ }
	}

	public class NotesDeleteAdminException : NotesException
	{
		public NotesDeleteAdminException()
			: base("The last administrator can't be deleted!")
		{ }
	}

	public class NotesModifyAdminException : NotesException
	{
		public NotesModifyAdminException()
			: base("The last administrator's role or state can't be changed!")
		{ }
	}

	public class NotesPasswordIncorrectException : NotesException
	{
		public NotesPasswordIncorrectException()
			: base("Old password isn't valid!")
		{ }
	}

	public class NotesLoginFailedException : NotesException
	{
		public NotesLoginFailedException()
			: base("Incorrect username or password!")
		{ }
	}

	public class NotesInvalidTokenException : NotesException
	{
		public NotesInvalidTokenException()
			: base("It looks like you clicked on an invalid password reset link. Please try again.")
		{ }
	}

	public class NotesNoteNotFoundException : NotesException
	{
		public NotesNoteNotFoundException()
			: base("Note not found!")
		{ }
	}

	public class NotesUserNotFoundException : NotesException
	{
		public NotesUserNotFoundException()
			: base("User not found!")
		{ }
	}

	public class NotesRevisionNotFoundException : NotesException
	{
		public NotesRevisionNotFoundException()
			: base("Revision not found!")
		{ }
	}
}