using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Configuration
{
    public class CachingOptions
    {
        [Required(ErrorMessage = "You must provide a book cache preservation duration in minutes")]
        [Range(0, int.MaxValue, ErrorMessage = "Book preservation duration cannot be a negative number")]
        public int BookTimespan { get; set; }
        [Required(ErrorMessage = "You must provide a collection cache preservation duration in minutes")]
        [Range(0, int.MaxValue, ErrorMessage = "Collection preservation duration cannot be a negative number")]
        public int CollectionTimespan { get; set; }
    }
}
