using FluentValidation;
using notes.Models;

namespace notes.Validators
{
	public class NotePostModelValidator : AbstractValidator<NotePostModel>
	{
		public NotePostModelValidator()
		{
			RuleFor(r => r.Title)
				.NotEmpty()
				.WithMessage("This field is required.")
				.MaximumLength(100)
				.WithMessage("Please enter no more than {MaxLength} characters.");

			RuleFor(r => r.Content)
				.NotEmpty()
				.WithMessage("This field is required.");

			RuleFor(r => r.Notebook)
				.MaximumLength(50)
				.WithMessage("Please enter no more than {MaxLength} characters.");

			RuleFor(r => r.Tags)
				.MaximumLengthInArrayString(50, ' ')
				.WithMessage("Tag too long.");
		}
	}
}