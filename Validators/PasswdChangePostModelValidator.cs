using FluentValidation;
using notes.Models;

namespace notes.Validators
{
	public class PasswdChangePostModelValidator : AbstractValidator<PasswdChangePostModel>
	{
		public PasswdChangePostModelValidator()
		{
			RuleFor(r => r.OldPassword)
				.NotEmpty()
				.WithMessage("Please fill in a password.");

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