public interface IPasswordPolicy
{
	bool IsValid(string password);
}