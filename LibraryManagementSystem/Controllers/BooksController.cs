using AutoMapper;
using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.ModelBinders;
using LMS_BusinessLogic;
using LMS_BusinessLogic.Interfaces;
using LMS_BusinessLogic.Models;
using LMS_Shared;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;
        private readonly ILogger<BooksController> _logger;
        public BooksController(IBookService bookService, IMapper mapper, ILogger<BooksController> logger)
        {
            _bookService = bookService;
            _mapper = mapper;
            _logger = logger;
        }

        //GET method, which retrieves all books from the library
        [HttpGet(Name = "ReadAllBooks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BookDTO>>> ReadAllBooks
        ([FromQuery] BookFiltersModel filters)
        {
            var booksModel = await _bookService.GetAllAsync(filters);
            var booksDTO = _mapper.Map<List<BookDTO>>(booksModel);

            _logger.LogInformation("Reading all books...");
            return Ok(booksDTO);
        }

        //GET method, which retrieves the book with the specified id
        [HttpGet("{id:int}", Name = "ReadBook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookDTO>> ReadBook(int id)
        {
            var result = await _bookService.ValidateExistingModel(id);
            if (result.OutModel == null)
            {
                _logger.LogError("ReadBook error: Cannot read the book with ID={0}: {1}", id, result.Message);
                return CheckFailedValidation(result.ValidationCode, result.Message);
            }
            var bookDTO = _mapper.Map<BookDTO>(result.OutModel);

            _logger.LogInformation("Reading one book with ID={0}...", id);
            return Ok(bookDTO);
        }

        //Private method, which checks a validation result code
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

        //GET method that retrieves the first occurrence of a book with the same author and title using a URL query and not a JSON body.
        [HttpGet("copies", Name = "ReadFirstCopyWithQuery")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookDTO>> ReadFirstCopyWithQuery([ModelBinder(BinderType = typeof(BookModelBinder))] BookDTO bookDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("ReadFirstCopyWithQuery error: Invalid BookDTO format in a URL query");
                return BadRequest($"Invalid book format");
            }
            var books = await _bookService.GetAllAsync(new BookFiltersModel());
            var copy = books.FirstOrDefault(c => string.Equals(c.Author, bookDTO.Author, StringComparison.OrdinalIgnoreCase)
                                            && string.Equals(c.Title, bookDTO.Title, StringComparison.OrdinalIgnoreCase));
            if (copy == null)
            {
                _logger.LogError("ReadFirstCopyWithQuery error: Cannot find the book from the URL query in the library:\nTitle:{0}, Author:{1}, Genre:{2}",
                    bookDTO.Title, bookDTO.Author, bookDTO.Genre);
                return NotFound($"There is no copy of provided book");
            }
            _logger.LogInformation("Found a book from the URL query:\nTitle:{0}, Author:{1}, Genre:{2}",
                    bookDTO.Title, bookDTO.Author, bookDTO.Genre);
            return Ok(copy);
        }

        //GET method that checks whether the book is borrowed or not
        [HttpGet("{id:int}/status", Name = "ReadBookStatus")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<bool>> IsBorrowed(int id)
        {
            var result = await _bookService.ValidateExistingModel(id);
            if (result.OutModel == null)
            {
                _logger.LogError("ReadBook error: Cannot read the book with ID={0}: {1}", id, result.Message);
                return CheckFailedValidation(result.ValidationCode, result.Message);
            }
            var bookDTO = _mapper.Map<BookDTO>(result.OutModel);
            var isBorrowed = bookDTO.Status == BookStatus.Borrowed;

            _logger.LogInformation("Book with ID={0} is borrowed: {1}", id, isBorrowed);
            return isBorrowed;
        }

        //POST method that creates a book from a JSON body
        [HttpPost(Name = "CreateBook")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BookDTO>> CreateBook([FromBody] BookOperationsDTO createdBookDTO)
        {
            if (createdBookDTO == null || !ModelState.IsValid)
            {
                _logger.LogError("CreateBook error: Invalid BookOpeationsDTO format");
                return BadRequest(createdBookDTO);
            }
            var bookModel = _mapper.Map<BookModel>(createdBookDTO);
            var createdModel = await _bookService.CreateAsync(bookModel);
            var bookDTO = _mapper.Map<BookDTO>(createdModel);

            _logger.LogInformation("Book with ID={0} has been successfully created", bookDTO.Id);
            return CreatedAtRoute("ReadBook", new { id = bookDTO.Id }, bookDTO);
        }

        //DELETE method, which deletes a book with the specified id
        [HttpDelete("{id:int}", Name = "DeleteBook")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var result = await _bookService.ValidateExistingModel(id);
            if (result.OutModel == null)
            {
                _logger.LogError("ReadBook error: Cannot read the book with ID={0}: {1}", id, result.Message);
                return CheckFailedValidation(result.ValidationCode, result.Message);
            }
            await _bookService.DeleteAsync(result.OutModel);

            _logger.LogInformation("Book with ID={0} has been successfully deleted", id);
            return NoContent();
        }

        //PUT method that updates the specified book with the specified id.
        [HttpPut("{id:int}", Name = "UpdateBook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookDTO>> UpdateBook(int id, [FromBody] BookOperationsDTO bookToUpdateDTO)
        {
            var result = await _bookService.ValidateExistingModel(id);
            if (result.OutModel == null)
            {
                _logger.LogError("ReadBook error: Cannot read the book with ID={0}: {1}", id, result.Message);
                return CheckFailedValidation(result.ValidationCode, result.Message);
            }
            if (bookToUpdateDTO == null)
            {
                _logger.LogError("UpdateBook error: Invalid BookOpeationsDTO format");
                return BadRequest($"Invalid update book format.");
            }
            var oldBookModel = await _bookService.GetAsync(result.OutModel.Id);
            var newBookModel = _mapper.Map<BookModel>(bookToUpdateDTO);
            newBookModel.CollectionId = oldBookModel.CollectionId;
            var updatedModel = await _bookService.UpdateAsync(newBookModel, id);

            _logger.LogInformation("Book with ID={0} has been successfully updated", id);
            return Ok(_mapper.Map<BookDTO>(updatedModel));
        }

        //PATCH method, which allows to change the status of a book with a specified id.
        [HttpPatch("{id:int}/status", Name = "UpdateBookStatus")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateBookStatus(int id, [FromBody] BookStatus? status)
        {
            var result = await _bookService.ValidateExistingModel(id);
            if (result.OutModel == null)
            {
                _logger.LogError("ReadBook error: Cannot read the book with ID={0}: {1}", id, result.Message);
                return CheckFailedValidation(result.ValidationCode, result.Message);
            }
            if (status == null)
            {
                _logger.LogError("UpdateBook error: Invalid status (status={0}", status);
                return BadRequest($"Invalid status (status=null)");
            }
            if (!Enum.IsDefined(typeof(BookStatus), status))
            {
                _logger.LogError("UpdateBook error: Invalid status (status={0}", status);
                return BadRequest($"Status={status} is not a valid book status");
            }
            var bookModel = result.OutModel;
            bookModel.Status = status.Value;
            await _bookService.UpdateAsync(bookModel, id);

            _logger.LogInformation("Status of book with ID={0} has been changed to status={1}", id, status);
            return NoContent();
        }
    }
}
