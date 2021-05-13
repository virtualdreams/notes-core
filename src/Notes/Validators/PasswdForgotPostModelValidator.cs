using FluentValidation;
using Notes.Models;

namespace Notes.Validators
{
	public class PasswdForgotPostModelValidator : AbstractValidator<PasswdForgotPostModel>
	{
		public PasswdForgotPostModelValidator()
		{
			RuleFor(r => r.Username)
				.NotEmpty()
				.WithMessage("Please fill in a username.")
				.EmailAddress()
				.WithMessage("Please enter a valid e-mail address.");
		}
	}
}