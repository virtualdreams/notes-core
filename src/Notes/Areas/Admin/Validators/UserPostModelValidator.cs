using FluentValidation;
using Notes.Areas.Admin.Models;
using Notes.FluentValidation;

namespace Notes.Areas.Admin.Validators
{
	public class UserPostModelValidator : AbstractValidator<UserPostModel>
	{
		private readonly IPasswordPolicy PasswordPolicy;

		public UserPostModelValidator(
			IPasswordPolicy passwordPolicy)
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