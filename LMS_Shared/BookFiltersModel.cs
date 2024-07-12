namespace LMS_Shared
{
    public class BookFiltersModel
    {
        public int? Year { get; set; } = null;
        public string? Title { get; set; } = null;
        public string? Author { get; set; } = null;
        public string? Genre { get; set; } = null;
        public BookStatus? Status { get; set; } = null;
        public int? CollectionId { get; set; } = null;
    }
}
