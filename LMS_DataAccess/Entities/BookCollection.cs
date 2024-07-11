namespace LMS_DataAccess.Entities
{
    public class BookCollection
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<Book> Books { get; set; } = new List<Book>();
    }
}
