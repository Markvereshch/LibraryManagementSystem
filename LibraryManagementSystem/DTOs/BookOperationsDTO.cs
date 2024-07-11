using LMS_Shared;

namespace LibraryManagementSystem.DTOs
{
    public class BookOperationsDTO //Class for creating and updating book models
    {
        public string Title { get; set; } = null!;
        public string Author { get; set; } = null!;
        public string Genre { get; set; } = null!;
        public int Year { get; set; }
        public BookStatus Status { get; set; } = 0;
    }
}
