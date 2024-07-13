using AutoMapper;
using LibraryManagementSystem.DTOs;
using LMS_BusinessLogic;
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
        private readonly ILogger<BookCollectionsController> _logger;
        public BookCollectionsController(IBookCollectionService bookCollection, IBookService bookService, IMapper mapper, ILogger<BookCollectionsController> logger)
        {
            _collectionService = bookCollection;
            _bookService = bookService;
            _mapper = mapper;
            _logger = logger;
        }

        //GET method, which retrieves all collections
        [HttpGet(Name = "ReadAllCollections")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BookCollectionDTO>>> ReadAllBookCollections()
        {
            var collectionsModel = await _collectionService.GetAllAsync();
            var collectionsDTO = _mapper.Map<List<BookCollectionDTO>>(collectionsModel);

            _logger.LogInformation("Reading all book collections...");
            return Ok(collectionsDTO);
        }

        //GET method, which retrieves the collection with the specified ID
        [HttpGet("{id:int}", Name = "ReadCollection")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookCollectionDTO>> ReadBookCollection(int id)
        {
            var result = await _collectionService.ValidateExistingModel(id);
            if (result.OutModel == null)
            {
                _logger.LogError("ReadBookCollection error: Cannot read the collection with ID={0}: {1}", id, result.Message);
                return CheckFailedValidation(result.ValidationCode, result.Message);
            }
            var collectionDTO = _mapper.Map<BookCollectionDTO>(result.OutModel);

            _logger.LogInformation("Reading one book collection with ID={0}...", id);
            return Ok(collectionDTO);
        }

        //Private method, which checks validation result code
        private ActionResult CheckFailedValidation(ValidationResults result, string message)
        {
            switch (result)
            {
                case ValidationResults.BadRequest:
                    return BadRequest(message);
                default:
                    return NotFound(message);
            }
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
                _logger.LogError("CreateBookCollection error: Invalid BookCollectionOpeationsDTO format");
                return BadRequest("Invalid collection format");
            }
            if (!await _collectionService.HasUniqueName(collectionDTO.Name))
            {
                _logger.LogError("CreateBookCollection error: There is already a collection with the name '{0}' in the library", collectionDTO.Name);
                return BadRequest("Such a collection already in the library");
            }
            var collectionModel = _mapper.Map<BookCollectionModel>(collectionDTO);
            var createdModel = await _collectionService.CreateAsync(collectionModel);
            var createdDTO = _mapper.Map<BookCollectionDTO>(createdModel);

            _logger.LogInformation("Collection with ID={0} has been successfully created", createdDTO.Id);
            return CreatedAtRoute("ReadCollection", new { id = createdDTO.Id }, createdDTO);
        }

        //DELETE method, which deletes the collection with the specified ID
        [HttpDelete("{id:int}", Name = "DeleteBookCollection")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBookCollection(int id)
        {
            var result = await _collectionService.ValidateExistingModel(id);
            if (result.OutModel == null)
            {
                _logger.LogError("DeleteBookCollection error: Cannot read the collection with ID={0}: {1}", id, result.Message);
                return CheckFailedValidation(result.ValidationCode, result.Message);
            }
            await _collectionService.DeleteAsync(result.OutModel);

            _logger.LogInformation("Book collection with ID={0} has been successfully deleted", id);
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
            var result = await _collectionService.ValidateExistingModel(id);
            if (result.OutModel == null)
            {
                _logger.LogError("ReadBookCollection error: Cannot read the collection with ID={0}: {1}", id, result.Message);
                return CheckFailedValidation(result.ValidationCode, result.Message);
            }
            if (bookId <= 0)
            {
                _logger.LogError("AssignBookToCollection error: Invalid book id={0}", bookId);
                return BadRequest($"Invalid book ID (bookId={bookId})");
            }
            var bookModel = await _bookService.GetAsync(bookId);
            if (bookModel == null)
            {
                _logger.LogError("AssignBookToCollection error: Book with ID={0} doesn't exist", bookId);
                return NotFound($"Book with ID={bookId} doesn't exist");
            }
            if (bookModel.CollectionId != null)
            {
                _logger.LogError("AssignBookToCollection error: Book with ID={0} is already assigned to the collection with ID={1}", bookId, bookModel.CollectionId);
                return Conflict($"Book with ID={bookId} is assigned to the collection with ID={bookModel.CollectionId}");
            }
            bookModel.CollectionId = id;
            await _bookService.UpdateAsync(bookModel, bookId);

            _logger.LogInformation("Book with ID={0} has been successfully assigned to the collection with ID={1}", bookId, id);
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
            var result = await _collectionService.ValidateExistingModel(id);
            if (result.OutModel == null)
            {
                _logger.LogError("ReadBookCollection error: Cannot read the collection with ID={0}: {1}", id, result.Message);
                return CheckFailedValidation(result.ValidationCode, result.Message);
            }
            if (bookId <= 0)
            {
                _logger.LogError("RemoveBookFromCollection error: Invalid book id={0}", bookId);
                return BadRequest($"Invalid book ID (bookId={bookId})");
            }
            var bookModel = await _bookService.GetAsync(bookId);
            if (bookModel == null || bookModel.CollectionId != id)
            {
                _logger.LogError("RemoveBookFromCollection error: Book with ID={0} is not assigned to the collection with ID={1}", bookId, id);
                return NotFound($"Book with ID={bookId} is not assigned to the collection with ID={id}");
            }
            bookModel.CollectionId = null;
            await _bookService.UpdateAsync(bookModel, bookId);

            _logger.LogInformation("Book with ID={0} has been successfully removed from the collection with ID={1}", bookId, id);
            return NoContent();
        }

        //PUT method that updates the name of the collection
        [HttpPut("{id:int}", Name = "UpdateCollection")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookCollectionDTO>> UpdateBookCollection(int id, [FromBody] BookCollectionOperationsDTO collectionDTO)
        {
            if (!await _collectionService.HasUniqueName(collectionDTO.Name))
            {
                _logger.LogError("CreateBookCollection error: There is already a collection with the name '{0}' in the library", collectionDTO.Name);
                return BadRequest("Such a collection already in the library");
            }
            var result = await _collectionService.ValidateExistingModel(id);
            if (result.OutModel == null)
            {
                _logger.LogError("ReadBookCollection error: Cannot read the collection with ID={0}: {1}", id, result.Message);
                return CheckFailedValidation(result.ValidationCode, result.Message);
            }
            result.OutModel.Name = collectionDTO.Name;
            var updatedModel = await _collectionService.UpdateAsync(result.OutModel, id);

            _logger.LogInformation("Collection with ID={0} has been successfully updated", id);
            return Ok(_mapper.Map<BookCollectionDTO>(updatedModel));
        }
    }
}
