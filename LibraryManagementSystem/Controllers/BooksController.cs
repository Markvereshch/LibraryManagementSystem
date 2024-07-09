using AutoMapper;
using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.ModelBinders;
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
        public BooksController(IBookService bookService, IMapper mapper)
        {
            _bookService = bookService;
            _mapper = mapper;
        }

        //GET method, which retrieves all books from the library
        [HttpGet(Name = "ReadAllBooks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BookDTO>>> ReadAllBooks
        ([FromQuery(Name = "nameFilter")] int? year,
        [FromQuery(Name = "titleFilter")] string? title,
        [FromQuery(Name = "authorFilter")] string? author,
        [FromQuery(Name = "genreFilter")] string? genre,
        [FromQuery(Name = "bookStatusFilter")] BookStatus? status,
        [FromQuery(Name = "collectionIdFilter")] int? collectionId)
        {
            var booksModel = await _bookService.GetAllBooksAsync(year, title, author, genre, status, collectionId);
            var booksDTO = _mapper.Map<List<BookDTO>>(booksModel);
            return Ok(booksDTO);
        }

        //GET method, which retrieves the book with the specified id
        [HttpGet("{id:int}", Name = "ReadBook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookDTO>> ReadBook(int id)
        {
            var result = await ValidateBook(id);
            if (result.Result != null)
            {
                return result.Result;
            }
            var bookDTO = result.Value;

            return Ok(bookDTO);
        }

        //Private helper method to validate the book
        private async Task<ActionResult<BookDTO>> ValidateBook(int id)
        {
            if (id <= 0)
            {
                return BadRequest($"Invalid book ID (id={id})");
            }
            var bookModel = await _bookService.GetBookAsync(id);
            if (bookModel == null)
            {
                return NotFound($"Book with ID={id} doesn't exist");
            }
            return _mapper.Map<BookDTO>(bookModel);
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
                return BadRequest($"Invalid book format");
            }
            var books = await _bookService.GetAllBooksAsync();
            var copy = books.FirstOrDefault(c => string.Equals(c.Author, bookDTO.Author, StringComparison.OrdinalIgnoreCase)
                                            && string.Equals(c.Title, bookDTO.Title, StringComparison.OrdinalIgnoreCase));
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
        public async Task<ActionResult<bool>> IsBorrowed(int id)
        {
            var result = await ValidateBook(id);
            if (result.Result != null)
            {
                return result.Result;
            }
            var bookDTO = result.Value;
            return bookDTO.Status == BookStatus.Borrowed;
        }

        //POST method that creates a book from a JSON body
        [HttpPost(Name = "CreateBook")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BookDTO>> CreateBook([FromBody] BookOperationsDTO createdBookDTO)
        {
            if (createdBookDTO == null)
            {
                return BadRequest(createdBookDTO);
            }
            var bookModel = _mapper.Map<BookModel>(createdBookDTO);
            var createdModel = await _bookService.CreateBookAsync(bookModel);
            var bookDTO = _mapper.Map<BookDTO>(createdModel);

            return CreatedAtRoute("ReadBook", new { id = bookDTO.Id }, bookDTO);
        }

        //DELETE method, which deletes a book with the specified id
        [HttpDelete("{id:int}", Name = "DeleteBook")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var result = await ValidateBook(id);
            if (result.Result != null)
            {
                return result.Result;
            }
            var bookDTO = result.Value;
            var bookModel = _mapper.Map<BookModel>(bookDTO);
            await _bookService.DeleteBookAsync(bookModel);

            return NoContent();
        }

        //PUT method that updates the specified book with the specified id.
        [HttpPut("{id:int}", Name = "UpdateBook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookDTO>> UpdateBook(int id, [FromBody] BookOperationsDTO bookToUpdateDTO)
        {
            var result = await ValidateBook(id);
            if (result.Result != null)
            {
                return result.Result;
            }
            if (bookToUpdateDTO == null)
            {
                return BadRequest($"Invalid update book format.");
            }
            var oldBookModel = await _bookService.GetBookAsync(result.Value.Id);
            var newBookModel = _mapper.Map<BookModel>(bookToUpdateDTO);
            newBookModel.CollectionId = oldBookModel.CollectionId;
            var updatedModel = await _bookService.UpdateBookAsync(newBookModel, id);

            return Ok(_mapper.Map<BookDTO>(updatedModel));
        }

        //PATCH method, which allows to change the status of a book with a specified id.
        [HttpPatch("{id:int}/status", Name = "UpdateBookStatus")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateBookStatus(int id, [FromBody] BookStatus? status)
        {
            var result = await ValidateBook(id);
            if (result.Result != null)
            {
                return result.Result;
            }
            if (status == null)
            {
                return BadRequest($"Invalid status (status={status})");
            }
            var bookDTO = result.Value;
            if (!Enum.IsDefined(typeof(BookStatus), status))
            {
                return BadRequest($"Status={status} is not a valid book status");
            }
            bookDTO.Status = (BookStatus)status;
            var bookModel = _mapper.Map<BookModel>(bookDTO);
            await _bookService.UpdateBookAsync(bookModel, id);

            return NoContent();
        }
    }
}
