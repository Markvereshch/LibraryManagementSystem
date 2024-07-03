using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class BookCollection
    {
        #region Id
        [Key]
        public int Id { get; set; }
        #endregion
        #region Name
        [Required(ErrorMessage = "Collection cannot be without a name")]
        [MaxLength(100, ErrorMessage = "Max length of author's fullname is 100 chars")]
        public string Name { get; set; }
        #endregion
        #region Collection
        [Required(ErrorMessage = "Collection should has a colleciton")]
        public List<Book> Books { get; set; }
        #endregion
    }
}
