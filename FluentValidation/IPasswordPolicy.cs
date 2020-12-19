namespace notes.FluentValidation
{
	public interface IPasswordPolicy
	{
		bool IsValid(string password);
	}
}