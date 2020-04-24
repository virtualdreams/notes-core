using FluentValidation;
using notes.Models;

namespace notes.Validators
{
	public class NoteTrashPostModelValidator : AbstractValidator<NoteTrashPostModel>
	{
		public NoteTrashPostModelValidator()
		{
			RuleFor(r => r.Id)
				.ListMustNotEmpty()
				.WithMessage("Selection must not empty.");
		}
	}
}