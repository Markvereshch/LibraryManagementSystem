namespace LMS_BusinessLogic.Models
{
    public class BookCollectionModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<BookModel> Books { get; set; }
    }
}
