using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTOs
{
    public class BookCollectionOperationsDTO
    {
        [Required(ErrorMessage = "Collection cannot be without a name")]
        [MaxLength(100, ErrorMessage = "Max length of author's fullname is 100 chars")]
        public string Name { get; set; }
    }
}
