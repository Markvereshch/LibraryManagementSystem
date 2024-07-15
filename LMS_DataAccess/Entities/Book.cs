using LMS_DataAccess.Interfaces;
using LMS_Shared;

namespace LMS_DataAccess.Entities
{
    public class Book : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Author { get; set; } = null!;
        public string Genre { get; set; } = null!;
        public int Year { get; set; }
        public BookStatus Status { get; set; } = 0;
        public int? CollectionId { get; set; }
        public BookCollection Collection { get; set; }
    }
}
