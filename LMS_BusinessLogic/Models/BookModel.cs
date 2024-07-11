using LMS_Shared;

namespace LMS_BusinessLogic.Models
{
    public class BookModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public int Year { get; set; }
        public BookStatus Status { get; set; }
        public int? CollectionId { get; set; }
    }
}