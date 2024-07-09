using LMS_Shared;
using System.ComponentModel.DataAnnotations;

namespace LMS_BusinessLogic.Models
{
    public class BookModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Enter a title")]
        [MaxLength(100, ErrorMessage = "Max length of a title is 100 chars")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Enter an author")]
        [MaxLength(100, ErrorMessage = "Max length of author's fullname is 100 chars")]
        [RegularExpression(@"^[A-Z][a-zA-Z'’]*(?: [A-Z][a-zA-Z'’]*(-[A-Z][a-zA-Z'’]*)*)+$", ErrorMessage = "Invalid full name format.")]
        public string Author { get; set; }
        [Required(ErrorMessage = "Enter a genre")]
        [RegularExpression(@"^[\p{L}\p{N}\p{P}\p{S}\s]+$", ErrorMessage = "Invalid genre format.")]
        [MaxLength(100, ErrorMessage = "Max length of a genre is 100 chars")]
        public string Genre { get; set; }
        [Required(ErrorMessage = "Enter a year")]
        [Range(0, 2024, ErrorMessage = "Year must be in range (0,2024)")]
        public int Year { get; set; }
        public BookStatus Status { get; set; }
        [Required(ErrorMessage = "Book must be either in a collection or not")]
        [Range(0, int.MaxValue, ErrorMessage = "CollectionId must be greater or equal to 0")]
        public int? CollectionId { get; set; }
    }
}