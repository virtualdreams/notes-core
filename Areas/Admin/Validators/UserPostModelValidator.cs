using FluentValidation;
using FluentValidation.Validators;
using notes.Areas.Admin.Models;

namespace notes.Areas.Admin.Validators
{
	public class UserPostModelValidator : AbstractValidator<UserPostModel>
	{
		public UserPostModelValidator()
		{
			RuleFor(r => r.Username)
				.NotEmpty()
				.WithMessage("This field is required.")
				.MaximumLength(100)
				.WithMessage("Please enter no more than {MaxLength} characters.")
				.EmailAddress(EmailValidationMode.Net4xRegex)
				.WithMessage("Please enter a valid e-mail address.");

			RuleFor(r => r.Password);
			// PasswordValidator 8 chars

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