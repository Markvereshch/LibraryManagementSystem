using LibraryManagementSystem.Data;
using LibraryManagementSystem.ModelBinders;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/BooksController")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        #region Read book/books/status
        [HttpGet(Name = "ReadAllBooks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Book>> ReadAllBooks([FromQuery(Name = "nameFilter")] int? year,
                                                            [FromQuery(Name = "titleFilter")] string? title,
                                                            [FromQuery(Name = "authorFilter")] string? author,
                                                            [FromQuery(Name = "genreFilter")] string? genre,
                                                            [FromQuery(Name = "bookStatusFilter")] BookStatus? status)
        {
            IEnumerable<Book> books = LocalLibrary.Books;
            if(year >= 0)
            {
                books = LocalLibrary.Books.Where(b => b.Year == year);
            }
            if(title != null) 
            {
                books = books.Where(b => b.Title.ToLower().Contains(title.ToLower()));
            }
            if (author != null)
            {
                books = books.Where(b => b.Author.ToLower().Contains(author.ToLower()));
            }
            if (genre != null)
            {
                books = books.Where(b => b.Genre.ToLower().Contains(genre.ToLower()));
            }
            if (status != null)
            {
                books = books.Where(b => (BookStatus)b.Status == status);
            }
            return Ok(books);
        }
        [HttpGet("{id:int}",Name = "ReadBook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Book> ReadBook(int id)
        {
            if(id <= 0)
            {
                return BadRequest($"Invalid book Id (id={id})");
            }
            var book = LocalLibrary.Books.FirstOrDefault(b => b.Id == id);
            if(book == null)
            {
                return NotFound($"Book with id={id} doesn't exist");
            }
            return Ok(book);
        }
        [HttpGet("findCopy", Name = "ReadFirstCopyWithQuery")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Book> ReadFirstCopyWithQuery([ModelBinder(BinderType = typeof(BookModelBinder))] Book book)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest($"Invalid book format");
            }
            var books = LocalLibrary.Books;
            var copy = books.FirstOrDefault(c => c.Author.ToLower() == book.Author.ToLower() && c.Title.ToLower() == book.Title.ToLower());
            if(copy == null)
            {
                return NotFound($"There is no copy of provided book");
            }
            return Ok(copy);
        }
        [HttpGet("{id:int}/status", Name = "ReadBookStatus")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<bool> IsBorrowed(int id)
        {
            if(id <= 0)
            {
                return BadRequest($"Invalid book Id (id={id})");
            }
            var book = LocalLibrary.Books.FirstOrDefault(b => b.Id == id);
            if(book == null)
            {
                return NotFound($"Book with id={id} doesn't exist");
            }

            return (BookStatus)book.Status == BookStatus.Borrowed;
        }
        #endregion
        #region Create book
        [HttpPost(Name = "CreateBook")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<Book> CreateBook([FromBody] Book createdBook)
        {
            if (createdBook == null)
            {
                return BadRequest(createdBook);
            }
            if (createdBook.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            createdBook.Id = LocalLibrary.BooksId;
            createdBook.IsAssigned = false;

            LocalLibrary.Books.Add(createdBook);

            return CreatedAtRoute("ReadBook", new { id = createdBook.Id }, createdBook);
        }
        #endregion
        #region Delete book
        [HttpDelete("{id:int}", Name = "DeleteBook")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteBook(int id)
        {
            if(id <= 0)
            {
                return BadRequest($"Invalid book Id (id={id})");
            }
            var book = LocalLibrary.Books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return NotFound($"Book with id={id} doesn't exist");
            }
            DeleteBookFromCollections(book);
            LocalLibrary.Books.Remove(book);

            return NoContent();
        }
        private void DeleteBookFromCollections(Book book)
        {
            var collections = LocalLibrary.Collections;
            foreach(var collection in collections)
            {
                collection.Books.Remove(book);
            }
        }
        #endregion
        #region Update book partially or fully
        [HttpPut("{id:int}", Name = "UpdateBook")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateBook(int id, [FromBody] Book bookToUpdate)
        {
            if(bookToUpdate == null || id != bookToUpdate.Id)
            {
                return BadRequest($"Updated book doesn't exist or its id is not valid");
            }
            var book = LocalLibrary.Books.FirstOrDefault(b => b.Id == id);
            if(book == null)
            {
                return NotFound($"Book with id={id} doesn't exist");
            }
            book.Title = bookToUpdate.Title;
            book.Author = bookToUpdate.Author;
            book.Year = bookToUpdate.Year;
            book.Genre = bookToUpdate.Genre;
            book.Status = bookToUpdate.Status;
        
            return NoContent();
        }

        [HttpPatch("{id:int}/status", Name="UpdateBookStatus")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateBookStatus(int id, [FromBody] BookStatus? status)
        {
            if (id <= 0 || status == null)
            {
                return BadRequest("Invalid id or status");
            }
            var book = LocalLibrary.Books.FirstOrDefault(b => b.Id == id);
            if(book == null)
            {
                return BadRequest($"Book with id={id} doesn't exist");
            }
            if(!Enum.IsDefined(typeof(BookStatus), status))
            {
                return BadRequest($"Status={status} is not a valid book status");
            }
            book.Status = (int)status;

            return NoContent();
        }
        #endregion
    }
}
 