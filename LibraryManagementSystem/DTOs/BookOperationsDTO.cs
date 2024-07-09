using System.ComponentModel.DataAnnotations;
using LMS_Shared;

namespace LibraryManagementSystem.DTOs
{
    public class BookOperationsDTO //Class for creating and updating book models
    {
        [Required(ErrorMessage = "Enter a title")]
        [MaxLength(100, ErrorMessage = "Max length of a title is 100 chars")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Enter an author")]
        [MaxLength(100, ErrorMessage = "Max length of author's fullname is 100 chars")]
        [RegularExpression(@"^[A-Z][a-zA-Z'’]*(?: [A-Z][a-zA-Z'’]*(-[A-Z][a-zA-Z'’]*)*)+$", ErrorMessage = "Invalid full name format.")]
        public string Author { get; set; } = null!;

        [Required(ErrorMessage = "Enter a genre")]
        [RegularExpression(@"^[\p{L}\p{N}\p{P}\p{S}\s]+$", ErrorMessage = "Invalid genre format.")]
        [MaxLength(100, ErrorMessage = "Max length of a genre is 100 chars")]
        public string Genre { get; set; } = null!;

        [Required(ErrorMessage = "Enter a year")]
        [Range(0, 2024, ErrorMessage = "Year must be in range (0,2024)")]
        public int Year { get; set; }

        [Required]
        [Range(0, 2, ErrorMessage = "Correct statuses: 'Available' = 0, 'Borrowed' = 1, 'Lost' = 2")]
        public BookStatus Status { get; set; } = 0;
    }
}
