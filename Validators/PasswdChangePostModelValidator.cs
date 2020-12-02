using FluentValidation;
using notes.Models;

namespace notes.Validators
{
	public class PasswdChangePostModelValidator : AbstractValidator<PasswdChangePostModel>
	{
		private readonly IPasswordPolicy PasswordPolicy;

		public PasswdChangePostModelValidator(IPasswordPolicy passwordPolicy)
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