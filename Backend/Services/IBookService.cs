using Backend.Models;

namespace Backend.Services;

public interface IBookService
{
    Task<BaseResponse> GetBooksAsync(CancellationToken cancellationToken = default);
    Task<BaseResponse> GetBookByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<BaseResponse> CreateBookAsync(CreateBookRequest request, CancellationToken cancellationToken = default);
    Task<BaseResponse> UpdateBookAsync(int id, UpdateBookRequest request, CancellationToken cancellationToken = default);
    Task<BaseResponse> DeleteBookAsync(int id, CancellationToken cancellationToken = default);
}
