using LMS_DataAccess.Interfaces;

namespace LMS_DataAccess.Entities
{
    public class BookCollection : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<Book> Books { get; set; } = new List<Book>();
    }
}
