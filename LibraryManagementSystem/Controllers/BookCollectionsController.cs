using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/book-collections")]
    [ApiController]
    public class BookCollectionsController : ControllerBase
    {
        //GET method, which retrieves all collections
        [HttpGet(Name = "ReadAllCollections")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<BookCollection>> ReadAllBookCollections()
        {
            return Ok(LocalLibrary.Collections);
        }

        //GET method, which retrieves the collection with the specified ID
        [HttpGet("{id:int}", Name = "ReadCollection")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<BookCollection> ReadBookCollection(int id)
        {
            if (id <= 0)
            {
                return BadRequest($"Invalid collection ID (id={id})");
            }
            var collection = LocalLibrary.Collections.FirstOrDefault(bc => bc.Id == id);
            if (collection == null)
            {
                return NotFound($"There is no collection with ID={id}");
            }
            return Ok(collection);
        }

        //POST method, which creates the book collection
        [HttpPost(Name = "CreateBookCollection")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<BookCollection> CreateBookCollection([FromBody] BookCollection createdCollection)
        {
            if (createdCollection == null)
            {
                ModelState.AddModelError("CollectionFormatError", "Invalid collection format");
                return BadRequest(createdCollection);
            }
            if (LocalLibrary.Collections.Any(c => string.Equals(c.Name, createdCollection.Name, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("CollectionExistsError", "Such a collection already in the library");
                return BadRequest();
            }
            if (createdCollection.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            createdCollection.Id = LocalLibrary.CollectionsId;
            LocalLibrary.Collections.Add(createdCollection);

            return CreatedAtRoute("ReadCollection", new { id = createdCollection.Id }, createdCollection);
        }

        //DELETE method, which deletes the collection with the specified ID
        [HttpDelete("{id:int}", Name = "DeleteBookCollection")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteBookCollection(int id)
        {
            if (id <= 0)
            {
                return BadRequest($"Invalid collection ID (id={id})");
            }
            var collection = LocalLibrary.Collections.FirstOrDefault(c => c.Id == id);
            if (collection == null)
            {
                return NotFound($"Collection with ID={id} cannot be found");
            }
            foreach (var book in collection.Books)
            {
                book.CollectionId = 0;
            }
            LocalLibrary.Collections.Remove(collection);

            return NoContent();
        }

        //PATCH method, which assigns the book with the specified bookId to the collection with the specified id
        [HttpPatch("{id:int}/books/{bookId:int}/assigned-book", Name = "AssignBookToCollection")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult AssignBookToCollection(int id, int bookId)
        {
            if (id <= 0 || bookId <= 0)
            {
                return BadRequest($"Invalid collection ID or book ID (id={id}, bookId={bookId})");
            }
            var collection = LocalLibrary.Collections.FirstOrDefault(c => c.Id == id);
            if (collection == null)
            {
                return NotFound($"Collection with ID={id} doesn't exist");
            }
            var book = LocalLibrary.Books.FirstOrDefault(b => b.Id == bookId);
            if (book == null)
            {
                return NotFound($"Book with ID={bookId} doesn't exist");
            }
            if (book.CollectionId != 0)
            {
                return Conflict($"Book with ID={bookId} is assigned to the collection with ID={book.CollectionId}");
            }
            collection.Books.Add(book);
            book.CollectionId = collection.Id;

            return NoContent();
        }

        //PATCH method, which removes the book with the specified bookId from the collection with the specified id 
        [HttpPatch("{id:int}/books/{bookId:int}/removed-book", Name = "RemoveBookFromCollection")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult RemoveBookFromCollection(int id, int bookId)
        {
            if (id <= 0 || bookId <= 0)
            {
                return BadRequest($"Invalid collection ID or book ID (id={id}, bookId={bookId})");
            }
            var collection = LocalLibrary.Collections.FirstOrDefault(c => c.Id == id);
            if (collection == null)
            {
                return NotFound($"Collection with ID={id} doesn't exist");
            }
            var book = collection.Books.FirstOrDefault(b => b.Id == bookId);
            if (book == null)
            {
                return NotFound($"Book with ID={bookId} is not assigned to collection with ID={id}");
            }
            collection.Books.Remove(book);
            book.CollectionId = 0;

            return NoContent();
        }
    }
}
