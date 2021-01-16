using FluentValidation;
using Notes.Models;

namespace Notes.Validators
{
	public class NoteTrashPostModelValidator : AbstractValidator<NoteTrashPostModel>
	{
		public NoteTrashPostModelValidator()
		{
			RuleFor(r => r.Id)
				.NotEmpty()
				.WithMessage("Selection must not empty.");
		}
	}
}