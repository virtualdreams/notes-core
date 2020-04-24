using FluentValidation;
using notes.Models;

namespace notes.Validators
{
	public class PasswdResetPostModelValidator : AbstractValidator<PasswdResetPostModel>
	{
		public PasswdResetPostModelValidator()
		{
			RuleFor(r => r.NewPassword)
				.NotEmpty()
				.WithMessage("Please fill in a password.");
			// PasswordValidator 8 chars

			RuleFor(r => r.ConfirmPassword)
				.NotEmpty()
				.WithMessage("Please confirm the password.")
				.Equal(r => r.NewPassword)
				.WithMessage("The passwords doesn't match.");
			// PasswordValidator 8 chars
		}
	}
}