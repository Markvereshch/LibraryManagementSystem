using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class BookCollection
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Collection cannot be without a name")]
        [MaxLength(100, ErrorMessage = "Max length of author's fullname is 100 chars")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Collection should has a colleciton")]
        public List<Book> Books { get; set; }
    }
}
