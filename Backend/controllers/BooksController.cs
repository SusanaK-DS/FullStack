using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService) => _bookService = bookService;

    [HttpPost("getList")]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse>> GetAll(CancellationToken cancellationToken)
    {
        var response = await _bookService.GetBooksAsync(cancellationToken);
        return StatusCode((int)response.HttpStatus, response);
    }

    [HttpGet("getById/{id:int}")]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var response = await _bookService.GetBookByIdAsync(id, cancellationToken);
        return StatusCode((int)response.HttpStatus, response);
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResponse>> Create(
        [FromBody] CreateBookRequest dto,
        CancellationToken cancellationToken)
    {
        var response = await _bookService.CreateBookAsync(dto, cancellationToken);
        return StatusCode((int)response.HttpStatus, response);
    }

    [HttpPost("update/{id:int}")]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResponse>> Update(
        int id,
        [FromBody] UpdateBookRequest dto,
        CancellationToken cancellationToken)
    {
        var response = await _bookService.UpdateBookAsync(id, dto, cancellationToken);
        return StatusCode((int)response.HttpStatus, response);
    }

    [HttpPost("delete/{id:int}")]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResponse>> Delete(int id, CancellationToken cancellationToken)
    {
        var response = await _bookService.DeleteBookAsync(id, cancellationToken);
        return StatusCode((int)response.HttpStatus, response);
    }
}
