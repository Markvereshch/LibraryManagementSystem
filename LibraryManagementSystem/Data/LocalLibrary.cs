using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Data
{
    public static class LocalLibrary
    {
        private static List<Book> books;
        private static List<BookCollection> collections;
        public static List<Book> Books
        {
            get => books;
            set => books = value;
        }
        public static List<BookCollection> Collections
        {
            get => collections;
            set => collections = value;
        }
        static LocalLibrary()
        {
            //creating test books
            var b1 = new Book
            {
                Id = 1,
                Author = "Sergiusz Piasecki",
                Genre = "Adventure",
                Title = "Kochanek Wielkiej Niedzwiedzicy",
                Year = 1937,
                Status = 1
            };
            var b2 = new Book
            {
                Id = 2,
                Author = "Jaroslaw Hasek",
                Genre = "Adventure",
                Title = "Dobry wojak Szwejk",
                Year = 1921,
                Status = 0
            };
            var b3 = new Book
            {
                Id = 3,
                Author = "AA",
                Genre = "BB",
                Title = "SomeTitle",
                Year = 1921,
                Status = 2
            };
            books = new List<Book>() { b1, b2, b3 };

            //creating test collection
            collections = new List<BookCollection>();
            var testCollection = new BookCollection()
            {
                Id = 1,
                Name = "My collection",
                Books = new List<Book>()
                {
                    b1, b2, b3
                }
            };
            collections.Add(testCollection);
        }
    }
}
