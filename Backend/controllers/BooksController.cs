using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController : ControllerBase
{
    private readonly IBookRepository _books;

    public BooksController(IBookRepository books) => _books = books;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<Book>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<Book>>> GetAll(CancellationToken cancellationToken)
        => Ok(await _books.GetAllAsync(cancellationToken));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Book), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Book>> GetById(int id, CancellationToken cancellationToken)
    {
        var book = await _books.GetByIdAsync(id, cancellationToken);
        return book is null ? NotFound() : Ok(book);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Book), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Book>> Create(
        [FromBody] CreateBookRequest dto,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Author))
            return BadRequest(new { error = "Title and Author are required." });

        var book = await _books.CreateAsync(dto.Title.Trim(), dto.Author.Trim(), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Book), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Book>> Update(
        int id,
        [FromBody] UpdateBookRequest dto,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Author))
            return BadRequest(new { error = "Title and Author are required." });

        var book = await _books.UpdateAsync(id, dto.Title.Trim(), dto.Author.Trim(), cancellationToken);
        return book is null ? NotFound() : Ok(book);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        => await _books.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
}
