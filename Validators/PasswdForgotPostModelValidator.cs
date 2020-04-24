using FluentValidation.Validators;
using FluentValidation;
using notes.Models;

namespace notes.Validators
{
	public class PasswdForgotPostModelValidator : AbstractValidator<PasswdForgotPostModel>
	{
		public PasswdForgotPostModelValidator()
		{
			RuleFor(r => r.Username)
				.NotEmpty()
				.WithMessage("Please fill in a username.")
				.EmailAddress(EmailValidationMode.Net4xRegex)
				.WithMessage("Please enter a valid e-mail address.");
		}
	}
}