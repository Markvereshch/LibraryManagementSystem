using LMS_Shared;

namespace LibraryManagementSystem.DTOs
{
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Author { get; set; } = null!;
        public string Genre { get; set; } = null!;
        public int Year { get; set; }
        public BookStatus Status { get; set; } = 0;
        public int? CollectionId { get; set; }
    }
}
