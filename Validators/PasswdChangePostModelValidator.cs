using FluentValidation;
using notes.Core.Internal;
using notes.Models;

namespace notes.Validators
{
	public class PasswdChangePostModelValidator : AbstractValidator<PasswdChangePostModel>
	{
		private readonly PasswordPolicy PasswordPolicy;

		public PasswdChangePostModelValidator(PasswordPolicy passwordPolicy)
		{
			PasswordPolicy = passwordPolicy;

			RuleFor(r => r.OldPassword)
				.NotEmpty()
				.WithMessage("Please fill in a password.");

			RuleFor(r => r.NewPassword)
				.NotEmpty()
				.WithMessage("Please fill in a password.")
				.Equal(r => r.ConfirmPassword)
				.WithMessage("The passwords doesn't match.")
				.ValidatePasswordPolicy(PasswordPolicy)
				.WithMessage("The password does not meet the password policy requirements.");

			RuleFor(r => r.ConfirmPassword)
				.NotEmpty()
				.WithMessage("Please confirm the password.");
		}
	}
}