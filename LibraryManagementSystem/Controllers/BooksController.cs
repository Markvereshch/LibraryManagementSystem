using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LibraryManagementSystem.Controllers
{
    [Route("api/BooksController")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        #region Read one book/all books
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Book>> ReadAllBooks([FromQuery]int? year)
        {
            // return Ok(LocalLibrary.Books);
            IEnumerable<Book> books;
            if(year >= 0)
            {
                books = LocalLibrary.Books.Where(b => b.Year == year);
            }
            else
            {
                books = LocalLibrary.Books;
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
                return BadRequest();
            }
            var book = LocalLibrary.Books.FirstOrDefault(book => book.Id == id);
            if(book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }
        #endregion
        #region Create book
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<Book> CreateBook([FromBody] Book createdBook)
        {
            if (LocalLibrary.Books.FirstOrDefault(book => createdBook.Title.ToLower() == book.Title.ToLower()
                                                     && createdBook.Author.ToLower() == book.Author.ToLower()
                                                     && createdBook.Year == book.Year) != null)
            {
                ModelState.AddModelError("BookExistsError", "Such a book already in the library");
                return BadRequest();
            }
            if (createdBook == null)
            {
                return BadRequest(createdBook);
            }
            if (createdBook.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            createdBook.Id = LocalLibrary.Books.OrderByDescending(book => book.Id).FirstOrDefault().Id + 1;
            LocalLibrary.Books.Add(createdBook);
            return CreatedAtRoute("", new { id = createdBook.Id }, createdBook);
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
                return BadRequest();
            }
            var book = LocalLibrary.Books.FirstOrDefault(book => book.Id == id);
            if (book == null)
            {
                return NotFound();
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
                return BadRequest();
            }
            var book = LocalLibrary.Books.FirstOrDefault(book => book.Id == id);
            if(book == null)
            {
                return NotFound();
            }
            book.Title = bookToUpdate.Title;
            book.Author = bookToUpdate.Author;
            book.Year = bookToUpdate.Year;
            book.Genre = bookToUpdate.Genre;
            book.Status = bookToUpdate.Status;

            return NoContent();
        }
        #endregion
    }
}
