using FluentValidation;
using LibraryManagementSystem.DTOs;

namespace LibraryManagementSystem.FluentValidators
{
    public class BookCollectionOperationsDTOValidator : AbstractValidator<BookCollectionOperationsDTO>
    {
        public BookCollectionOperationsDTOValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Collection name max length is 100 chars");
        }
    }
}
