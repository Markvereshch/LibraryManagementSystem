﻿using LibraryManagementSystem.Data;
using LibraryManagementSystem.ModelBinders;
using LibraryManagementSystem.Models;
using LMS_BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization.Metadata;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }
        //GET method, which retrieves all books from the library
        [HttpGet(Name = "ReadAllBooks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Book>>> ReadAllBooks
        ([FromQuery(Name = "nameFilter")] int? year,
        [FromQuery(Name = "titleFilter")] string? title,
        [FromQuery(Name = "authorFilter")] string? author,
        [FromQuery(Name = "genreFilter")] string? genre,
        [FromQuery(Name = "bookStatusFilter")] BookStatus? status,
        [FromQuery(Name = "collectionIdFilter")] int? collectionId)
        {
            var books = await _bookService.GetAllBooksAsync(year, title, author, genre, (LMS_DataAccess.Data.BookStatus?)status, collectionId);
            return Ok(books);
        }
        //GET method, which retrieves the book with the specified id
        [HttpGet("{id:int}", Name = "ReadBook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Book>> ReadBook(int id)
        {
            var result = await ValidateBook(id);
            if (result.Result != null)
            {
                return result.Result;
            }
            var book = result.Value;

            return Ok(book);
        }
        //Private helper method to validate the book
        private async Task<ActionResult<Book>> ValidateBook(int id)
        {
            if (id <= 0)
            {
                return BadRequest($"Invalid book ID (id={id})");
            }
            var book = await _bookService.GetBookAsync(id);
            if (book == null)
            {
                return NotFound($"Book with ID={id} doesn't exist");
            }
            return Ok(book);
        }
        /*
        //GET method that retrieves the first occurrence of a book with the same author and title using a URL query and not a JSON body.
        [HttpGet("copies", Name = "ReadFirstCopyWithQuery")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Book> ReadFirstCopyWithQuery([ModelBinder(BinderType = typeof(BookModelBinder))] Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest($"Invalid book format");
            }
            var books = LocalLibrary.Books;
            var copy = books.FirstOrDefault(c => string.Equals(c.Author, book.Author, StringComparison.OrdinalIgnoreCase)
                                            && string.Equals(c.Title, book.Title, StringComparison.OrdinalIgnoreCase));
            if (copy == null)
            {
                return NotFound($"There is no copy of provided book");
            }
            return Ok(copy);
        }

        //GET method that checks whether the book is borrowed or not
        [HttpGet("{id:int}/status", Name = "ReadBookStatus")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<bool> IsBorrowed(int id)
        {
            var result = ValidateBook(id);
            if (result.Result != null)
            {
                return result.Result;
            }
            var book = result.Value;
            return book.Status == BookStatus.Borrowed;
        }
        */
        //POST method that creates a book from a JSON body
        [HttpPost(Name = "CreateBook")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Book>> CreateBook([FromBody] Book createdBook)
        {
            if (createdBook == null)
            {
                return BadRequest(createdBook);
            }
            if (createdBook.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            //var created = await _bookService.CreateBookAsync(createdBook);
            return CreatedAtRoute("ReadBook", new { id = createdBook.Id }, createdBook);
        }
        /*
        //DELETE method, which deletes a book with the specified id
        [HttpDelete("{id:int}", Name = "DeleteBook")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteBook(int id)
        {
            var result = ValidateBook(id);
            if (result.Result != null)
            {
                return result.Result;
            }
            var book = result.Value;
            DeleteBookFromCollections(book);
            LocalLibrary.Books.Remove(book);

            return NoContent();
        }

        //Private helper method, which removes all books from the collection we are about to delete.
        private void DeleteBookFromCollections(Book book)
        {
            var collections = LocalLibrary.Collections;
            foreach (var collection in collections)
            {
                collection.Books.Remove(book);
            }
        }

        //PUT method that updates the specified book with the specified id.
        [HttpPut("{id:int}", Name = "UpdateBook")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateBook(int id, [FromBody] Book bookToUpdate)
        {
            var result = ValidateBook(id);
            if (result.Result != null)
            {
                return result.Result;
            }
            if (bookToUpdate == null)
            {
                return BadRequest($"Invalid update book format.");
            }
            var book = result.Value;

            book.Title = bookToUpdate.Title;
            book.Author = bookToUpdate.Author;
            book.Year = bookToUpdate.Year;
            book.Genre = bookToUpdate.Genre;
            book.Status = bookToUpdate.Status;

            return NoContent();
        }

        //PATCH method, which allows to change the status of a book with a specified id.
        [HttpPatch("{id:int}/status", Name = "UpdateBookStatus")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateBookStatus(int id, [FromBody] BookStatus? status)
        {
            var result = ValidateBook(id);
            if (result.Result != null)
            {
                return result.Result;
            }
            if(status == null)
            {
                return BadRequest($"Invalid status (status={status})");
            }
            var book = result.Value;
            if (!Enum.IsDefined(typeof(BookStatus), status))
            {
                return BadRequest($"Status={status} is not a valid book status");
            }
            book.Status = (BookStatus)status;

            return NoContent();
        }
        */
    }
}
