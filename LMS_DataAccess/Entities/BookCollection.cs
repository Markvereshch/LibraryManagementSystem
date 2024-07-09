using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LMS_DataAccess.Entities
{
    public class BookCollection
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Collection cannot be without a name")]
        [MaxLength(100, ErrorMessage = "Max length of author's fullname is 100 chars")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Collection should has a colleciton")]
        public List<Book> Books { get; set; } = new List<Book>();
    }
}
