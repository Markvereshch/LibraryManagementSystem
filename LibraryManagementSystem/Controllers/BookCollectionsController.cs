using AutoMapper;
using LibraryManagementSystem.DTOs;
using LMS_BusinessLogic.Interfaces;
using LMS_BusinessLogic.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/book-collections")]
    [ApiController]
    public class BookCollectionsController : ControllerBase
    {

        private readonly IBookCollectionService _collectionService;
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;
        public BookCollectionsController(IBookCollectionService bookCollection, IBookService bookService, IMapper mapper)
        {
            _collectionService = bookCollection;
            _bookService = bookService;
            _mapper = mapper;
        }

        //GET method, which retrieves all collections
        [HttpGet(Name = "ReadAllCollections")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BookCollectionDTO>>> ReadAllBookCollections()
        {
            var collectionsModel = await _collectionService.GetAllCollectionsAsync();
            var collectionsDTO = _mapper.Map<List<BookCollectionDTO>>(collectionsModel);
            return Ok(collectionsDTO);
        }

        //GET method, which retrieves the collection with the specified ID
        [HttpGet("{id:int}", Name = "ReadCollection")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookCollectionDTO>> ReadBookCollection(int id)
        {
            var result = await ValidateCollection(id);
            if (result.Result != null)
            {
                return result.Result;
            }
            var collectionDTO = result.Value;

            return Ok(collectionDTO);
        }

        //Private helper method to validate the collection
        private async Task<ActionResult<BookCollectionDTO>> ValidateCollection(int id)
        {
            if (id <= 0)
            {
                return BadRequest($"Invalid collection ID (id={id})");
            }
            var collectionModel = await _collectionService.GetBookCollectionAsync(id);
            if (collectionModel == null)
            {
                return NotFound($"Collection with ID={id} cannot be found");
            }
            return _mapper.Map<BookCollectionDTO>(collectionModel);
        }

        //POST method, which creates the book collection
        [HttpPost(Name = "CreateBookCollection")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BookCollectionDTO>> CreateBookCollection([FromBody] BookCollectionOperationsDTO collectionDTO)
        {
            if (collectionDTO == null)
            {
                return BadRequest("Invalid collection format");
            }
            var collections = await _collectionService.GetAllCollectionsAsync();
            if (collections.Any(c => string.Equals(c.Name, collectionDTO.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest("Such a collection already in the library");
            }
            var collectionModel = _mapper.Map<BookCollectionModel>(collectionDTO);
            var createdModel = await _collectionService.CreateBookCollectionAsync(collectionModel);
            var createdDTO = _mapper.Map<BookCollectionDTO>(createdModel);

            return CreatedAtRoute("ReadCollection", new { id = createdDTO.Id }, createdDTO);
        }

        //DELETE method, which deletes the collection with the specified ID
        [HttpDelete("{id:int}", Name = "DeleteBookCollection")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBookCollection(int id)
        {
            var result = await ValidateCollection(id);
            if (result.Result != null)
            {
                return result.Result;
            }
            var collectionDTO = result.Value;
            var collectionModel = _mapper.Map<BookCollectionModel>(collectionDTO);
            await _collectionService.DeleteBookCollectionAsync(collectionModel);

            return NoContent();
        }

        //PATCH method, which assigns the book with the specified bookId to the collection with the specified id
        [HttpPatch("{id:int}/books/{bookId:int}/assigned-book", Name = "AssignBookToCollection")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AssignBookToCollection(int id, int bookId)
        {
            var result = await ValidateCollection(id);
            if (result.Result != null)
            {
                return result.Result;
            }
            if (bookId <= 0)
            {
                return BadRequest($"Invalid book ID (bookId={bookId})");
            }
            var bookModel = await _bookService.GetBookAsync(bookId);
            if (bookModel == null)
            {
                return NotFound($"Book with ID={bookId} doesn't exist");
            }
            if (bookModel.CollectionId != null)
            {
                return Conflict($"Book with ID={bookId} is assigned to the collection with ID={bookModel.CollectionId}");
            }
            bookModel.CollectionId = id;
            await _bookService.UpdateBookAsync(bookModel, bookId);

            return NoContent();
        }

        //PATCH method, which removes the book with the specified bookId from the collection with the specified id 
        [HttpPatch("{id:int}/books/{bookId:int}/removed-book", Name = "RemoveBookFromCollection")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RemoveBookFromCollection(int id, int bookId)
        {
            var result = await ValidateCollection(id);
            if (result.Result != null)
            {
                return result.Result;
            }
            if (bookId <= 0)
            {
                return BadRequest($"Invalid book ID (bookId={bookId})");
            }
            var bookModel = await _bookService.GetBookAsync(bookId);
            if (bookModel == null || bookModel.CollectionId != id)
            {
                return NotFound($"Book with ID={bookId} is not assigned to collection with ID={id}");
            }
            bookModel.CollectionId = null;
            await _bookService.UpdateBookAsync(bookModel, bookId);

            return NoContent();
        }
    }
}
