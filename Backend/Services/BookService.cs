using Backend.Data;
using Backend.Models;
using System.Net;

namespace Backend.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<BaseResponse> GetBooksAsync(CancellationToken cancellationToken = default)
    {
        var books = await _bookRepository.GetAllAsync(cancellationToken);
        return new BaseResponse
        {
            Data = books
        };
    }

    public async Task<BaseResponse> GetBookByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var book = await _bookRepository.GetByIdAsync(id, cancellationToken);
        if (book is null)
        {
            return new BaseResponse
            {
                Status = false,
                ErrorCode = 0,
                ErrorMessage = "Book not found.",
                HttpStatus = HttpStatusCode.NotFound
            };
        }

        return new BaseResponse
        {
            Data = book
        };
    }

    public async Task<BaseResponse> CreateBookAsync(CreateBookRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Author))
        {
            return new BaseResponse
            {
                Status = false,
                ErrorCode = 0,
                ErrorMessage = "Title and Author are required.",
                HttpStatus = HttpStatusCode.BadRequest
            };
        }

        var created = await _bookRepository.CreateAsync(request.Title.Trim(), request.Author.Trim(), cancellationToken);
        return new BaseResponse
        {
            Data = created,
            HttpStatus = HttpStatusCode.Created
        };
    }

    public async Task<BaseResponse> UpdateBookAsync(int id, UpdateBookRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Author))
        {
            return new BaseResponse
            {
                Status = false,
                ErrorCode = 0,
                ErrorMessage = "Title and Author are required.",
                HttpStatus = HttpStatusCode.BadRequest
            };
        }

        var updated = await _bookRepository.UpdateAsync(id, request.Title.Trim(), request.Author.Trim(), cancellationToken);
        if (updated is null)
        {
            return new BaseResponse
            {
                Status = false,
                ErrorCode = 0,
                ErrorMessage = "Book not found.",
                HttpStatus = HttpStatusCode.NotFound
            };
        }

        return new BaseResponse
        {
            Data = updated
        };
    }

    public async Task<BaseResponse> DeleteBookAsync(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _bookRepository.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return new BaseResponse
            {
                Status = false,
                ErrorCode = 0,
                ErrorMessage = "Book not found.",
                HttpStatus = HttpStatusCode.NotFound
            };
        }

        return new BaseResponse
        {
            Data = new { Message = "Book deleted successfully." }
        };
    }
}
