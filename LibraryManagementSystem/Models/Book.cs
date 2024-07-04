using LibraryManagementSystem.ModelBinders;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Book
    {
        #region Id
        [Key]
        public int Id { get; set; }
        #endregion
        #region Title
        [Required(ErrorMessage = "Enter a title")]
        [MaxLength(100, ErrorMessage = "Max length of a title is 100 chars")]
        public string Title { get; set; }
        #endregion
        #region Author
        [Required(ErrorMessage = "Enter an author")]
        [MaxLength(100, ErrorMessage = "Max length of author's fullname is 100 chars")]
        [RegularExpression(@"^[A-Z][a-zA-Z'’]*(?: [A-Z][a-zA-Z'’]*(-[A-Z][a-zA-Z'’]*)*)+$", ErrorMessage = "Invalid full name format.")]
        public string Author { get; set; }
        #endregion
        #region Genre
        [Required(ErrorMessage = "Enter a genre")]
        [RegularExpression(@"^[\p{L}\p{N}\p{P}\p{S}\s]+$", ErrorMessage = "Invalid genre format.")]
        [MaxLength(100, ErrorMessage = "Max length of a genre is 100 chars")]
        public string Genre { get; set; }
        #endregion
        #region Year
        [Required(ErrorMessage = "Enter a year")]
        [Range(0, 2024, ErrorMessage = "Year must be in range (0,2024)")]
        public int Year { get; set; }
        #endregion
        #region Status
        [Range(0, 2, ErrorMessage = "Correct statuses: 'Available' = 0, 'Borrowed' = 1, 'Lost' = 2")]
        public int Status { get; set; }
        #endregion
        #region IsAssigned
        [Required(ErrorMessage = "Book must be either in a collection or not")]
        public bool IsAssigned { get; set; }
        #endregion
    }
}