namespace Notes.FluentValidation
{
	public interface IPasswordPolicy
	{
		bool IsValid(string password);
	}
}