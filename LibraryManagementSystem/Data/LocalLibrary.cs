using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Data
{
    public static class LocalLibrary
    {
        private static List<Book> _books;
        private static List<BookCollection> _collections;
        private static int _booksId = 0;
        private static int _collectionsId = 0;
        public static int BooksId
        {
            get => ++_booksId;
        }
        public static int CollectionsId
        {
            get => ++_collectionsId;
        }
        public static List<Book> Books
        {
            get => _books;
            set => _books = value;
        }
        public static List<BookCollection> Collections
        {
            get => _collections;
            set => _collections = value;
        }
        static LocalLibrary()
        {
            //creating test books
            var testBook1 = new Book
            {
                Id = BooksId,
                Author = "Sergiusz Piasecki",
                Genre = "Adventure",
                Title = "Kochanek Wielkiej Niedzwiedzicy",
                Year = 1937,
                Status = BookStatus.Borrowed,
                CollectionId = 1
            };
            var testBook2 = new Book
            {
                Id = BooksId,
                Author = "Jaroslaw Hasek",
                Genre = "Adventure",
                Title = "Dobry wojak Szwejk",
                Year = 1921,
                Status = BookStatus.Available,
                CollectionId = 1
            };
            var testBook3 = new Book
            {
                Id = BooksId,
                Title = "1984",
                Author = "George Orwell",
                Genre = "Dystopian",
                Year = 1949,
                Status = BookStatus.Available,
                CollectionId = 0
            };
            _books = new List<Book>() { testBook1, testBook2, testBook3 };

            //creating a test collection
            _collections = new List<BookCollection>();
            var testCollection = new BookCollection()
            {
                Id = CollectionsId,
                Name = "My collection",
                Books = new List<Book>()
                {
                    testBook1, testBook2
                }
            };
            _collections.Add(testCollection);
        }
    }
}
