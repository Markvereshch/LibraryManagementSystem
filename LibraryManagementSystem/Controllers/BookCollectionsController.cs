using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/BookCollectionsController")]
    [ApiController]
    public class BookCollectionsController : ControllerBase
    {
        #region Read one collection or all
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<BookCollection>> ReadAllBookCollections()
        {
            return Ok(LocalLibrary.Collections);
        }

        [HttpGet("{id:int}", Name = "ReadCollection")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<BookCollection> ReadBookCollection(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            var collection = LocalLibrary.Collections.FirstOrDefault(bc => bc.Id == id);
            if (collection == null)
            {
                return NotFound();
            }
            return Ok(collection);
        }
        #endregion
        #region Create collection
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<BookCollection> CreateBookCollection([FromBody] BookCollection createdCollection)
        {
            if (LocalLibrary.Collections.FirstOrDefault(collection => createdCollection.Name == collection.Name) != null)
            {
                ModelState.AddModelError("CollectionExistsError", "Such a collection collection already in the library");
                return BadRequest();
            }
            if (createdCollection == null)
            {
                return BadRequest(createdCollection);
            }
            if (createdCollection.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            createdCollection.Id = LocalLibrary.Collections.OrderByDescending(collection => collection.Id).FirstOrDefault().Id + 1;
            LocalLibrary.Collections.Add(createdCollection);
            return CreatedAtRoute("", new { id = createdCollection.Id }, createdCollection);
        }
        #endregion
        #region Delete collection
        [HttpDelete("{id:int}", Name = "DeleteBookCollection")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteBookCollection(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            var collection = LocalLibrary.Collections.FirstOrDefault(collection => collection.Id == id);
            if (collection == null)
            {
                return NotFound();
            }
            LocalLibrary.Collections.Remove(collection);
            return NoContent();
        }
        #endregion
        #region Update collection
        [HttpPut("{id:int}", Name = "UpdateBookCollection")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateBook(int id, [FromBody] BookCollection collectionToUpdate)
        {
            if (collectionToUpdate == null || id != collectionToUpdate.Id)
            {
                return BadRequest();
            }
            var collection = LocalLibrary.Collections.FirstOrDefault(book => book.Id == id);
            if (collection == null)
            {
                return NotFound();
            }
            collection.Name = collectionToUpdate.Name;
            collection.Books = collectionToUpdate.Books;

            return NoContent();
        }
        #endregion
    }
}
