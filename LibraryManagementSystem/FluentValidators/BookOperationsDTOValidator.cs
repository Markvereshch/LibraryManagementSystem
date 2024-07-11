using FluentValidation;
using LibraryManagementSystem.DTOs;
using System.Text.RegularExpressions;

namespace LibraryManagementSystem.FluentValidators
{
    public class BookOperationsDTOValidator : AbstractValidator<BookOperationsDTO>
    {
        public BookOperationsDTOValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required")
                .MaximumLength(100)
                .WithMessage("Max length is 100 chars");
            RuleFor(x => x.Author)
                .NotEmpty()
                .WithMessage("Title is required")
                .MaximumLength(100)
                .WithMessage("Max length is 100 chars")
                .Matches(new Regex(@"^[A-Z][a-zA-Z'’]*(?: [A-Z][a-zA-Z'’]*(-[A-Z][a-zA-Z'’]*)*)+$"))
                .WithMessage("Invalid full name format.");
            RuleFor(book => book.Genre)
                .NotEmpty()
                .WithMessage("Enter a genre")
                .MaximumLength(100)
                .WithMessage("Max length of a genre is 100 chars")
                .Matches(new Regex(@"^[\p{L}\p{N}\p{P}\p{S}\s]+$"))
                .WithMessage("Invalid genre format.");
            RuleFor(book => book.Year)
                .NotEmpty()
                .WithMessage("Enter a year")
                .InclusiveBetween(0, 2024)
                .WithMessage("Year must be in range (0,2024)");
            RuleFor(book => book.Status)
                .IsInEnum()
                .WithMessage("Invalid BookStatus value");
        }
    }
}
