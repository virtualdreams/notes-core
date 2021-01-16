using FluentValidation;
using Notes.Models;

namespace Notes.Validators
{
	public class LoginModelValidator : AbstractValidator<LoginModel>
	{
		public LoginModelValidator()
		{
			RuleFor(r => r.Username)
				.NotEmpty()
				.WithMessage("Please fill in a username.");

			RuleFor(r => r.Password)
				.NotEmpty()
				.WithMessage("Please fill in a password.");
		}
	}
}