using FluentValidation;
using FluentValidation.Validators;
using notes.Areas.Admin.Models;
using notes.Core.Internal;
using notes.Validators;

namespace notes.Areas.Admin.Validators
{
	public class UserPostModelValidator : AbstractValidator<UserPostModel>
	{
		private readonly PasswordPolicy PasswordPolicy;

		public UserPostModelValidator(PasswordPolicy passwordPolicy)
		{
			PasswordPolicy = passwordPolicy;

			RuleFor(r => r.Username)
				.NotEmpty()
				.WithMessage("This field is required.")
				.MaximumLength(100)
				.WithMessage("Please enter no more than {MaxLength} characters.")
				.EmailAddress()
				.WithMessage("Please enter a valid e-mail address.");

			RuleFor(r => r.Password);
			// .ValidatePasswordPolicy(PasswordPolicy)
			// .WithMessage("The password does not meet the password policy requirements.");

			RuleFor(r => r.DisplayName)
				.MaximumLength(50)
				.WithMessage("Please enter no more than {MaxLength} characters.");

			RuleFor(r => r.Role)
				.NotEmpty()
				.WithMessage("This field is required.")
				.MaximumLength(50)
				.WithMessage("Please enter no more than {MaxLength} characters.")
				.Matches("^Administrator|User$")
				.WithMessage("Invalid role.");
		}
	}
}