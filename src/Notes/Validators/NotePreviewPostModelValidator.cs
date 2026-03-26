using FluentValidation;
using Notes.FluentValidation;
using Notes.Models;

namespace Notes.Validators
{
	public class NotePreviewModelValidator : AbstractValidator<NotePreviewPostModel>
	{
		public NotePreviewModelValidator()
		{
			RuleFor(r => r.Content)
				.NotEmpty()
				.WithMessage("This field is required.");
		}
	}
}