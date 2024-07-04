﻿using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/BookCollectionsController")]
    [ApiController]
    public class BookCollectionsController : ControllerBase
    {
        #region Read one collection or all
        [HttpGet(Name = "ReadAllCollections")]
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
                return BadRequest($"Invalid Id (id = {id}");
            }
            var collection = LocalLibrary.Collections.FirstOrDefault(bc => bc.Id == id);
            if (collection == null)
            {
                return NotFound($"There is no collection with ID={id}");
            }
            return Ok(collection);
        }
        #endregion
        #region Create collection
        [HttpPost(Name = "CreateBookCollection")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<BookCollection> CreateBookCollection([FromBody] BookCollection createdCollection)
        {
            if (LocalLibrary.Collections.FirstOrDefault(c => createdCollection.Name == c.Name) != null)
            {
                ModelState.AddModelError("CollectionExistsError", "Such a collection already in the library");
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
            createdCollection.Id = LocalLibrary.CollectionsId;
            LocalLibrary.Collections.Add(createdCollection);

            return CreatedAtRoute("ReadCollection", new { id = createdCollection.Id }, createdCollection);
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
                return BadRequest($"Invalid ID (id = {id})");
            }
            var collection = LocalLibrary.Collections.FirstOrDefault(c => c.Id == id);
            if (collection == null)
            {
                return NotFound($"Collection with ID={id} cannot be found");
            }
            foreach (var book in collection.Books)
            {
                book.IsAssigned = false;
            }
            LocalLibrary.Collections.Remove(collection);
            return NoContent();
        }
        #endregion
        #region Update collection
        [HttpPatch("{id:int}/books/assign", Name = "AssignBookToCollection")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult AssignBookToCollection(int id, int bookId)
        {
            if(id <= 0 || bookId <= 0)
            {
                return BadRequest($"Invalid id or bookId (id={id}, bookId={id})");
            }
            var collection = LocalLibrary.Collections.FirstOrDefault(c => c.Id == id);
            if (collection == null)
            {
                return NotFound($"Collection with id={id} doesn't exist");
            }
            var book = LocalLibrary.Books.FirstOrDefault(b => b.Id == bookId);
            if (book == null)
            {
                return NotFound($"Book with bookId={bookId} doesn't exist");
            }
            if(book.IsAssigned)
            {
                return Conflict($"Book with id={bookId} is assigned to some collection");
            }
            collection.Books.Add(book);
            book.IsAssigned = true;

            return NoContent();
        }
        [HttpPatch("{id:int}/books/remove", Name = "RemoveBookFromCollection")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult RemoveBookFromCollection(int id, int bookId)
        {
            if (id <= 0 || bookId <= 0)
            {
                return BadRequest($"Invalid id or bookId (id={id}, bookId={id})");
            }
            var collection = LocalLibrary.Collections.FirstOrDefault(c => c.Id == id);
            if (collection == null)
            {
                return NotFound($"Collection with id={id} doesn't exist");
            }
            var book = collection.Books.FirstOrDefault(b => b.Id == bookId);
            if (book == null)
            {
                return NotFound($"Book with bookId={bookId} is not assigned to collection with id={id}");
            }
            collection.Books.Remove(book);
            book.IsAssigned = false;

            return NoContent();
        }
        #endregion
    }
}
