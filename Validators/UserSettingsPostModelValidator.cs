using FluentValidation;
using notes.Models;

namespace notes.Validators
{
	public class UserSettingsPostModelValidator : AbstractValidator<UserSettingsPostModel>
	{
		public UserSettingsPostModelValidator()
		{
			RuleFor(r => r.DisplayName)
				.MaximumLength(50)
				.WithMessage("Please enter no more than {MaxLength} characters.");

			RuleFor(r => r.Items)
				.NotEmpty()
				.WithMessage("This field is required.")
				.InclusiveBetween(1, 100)
				.WithMessage("Please enter a value between {From} to {To}.");
		}
	}
}