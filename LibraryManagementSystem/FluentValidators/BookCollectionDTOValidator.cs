using FluentValidation;
using LibraryManagementSystem.DTOs;

namespace LibraryManagementSystem.FluentValidators
{
    public class BookCollectionDTOValidator : AbstractValidator<BookCollectionDTO>
    {
        public BookCollectionDTOValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty()
                .WithMessage("Collection must have an ID");
            RuleFor(c => c.Name)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Collection name max length is 100 chars");
            RuleFor(c => c.Books)
                .NotEmpty()
                .WithMessage("Collection should have a collection");
        }
    }
}
