using Backend.Models;
using Dapper;
using Npgsql;

namespace Backend.Data;

public interface IBookRepository
{
    Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Book> CreateAsync(string title, string author, CancellationToken cancellationToken = default);
    Task<Book?> UpdateAsync(int id, string title, string author, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

public class BookRepository : IBookRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public BookRepository(NpgsqlDataSource dataSource) => _dataSource = dataSource;

    public async Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var conn = await _dataSource.OpenConnectionAsync(cancellationToken);
        var cmd = new CommandDefinition(
            "SELECT id, title, author FROM books ORDER BY id",
            cancellationToken: cancellationToken);
        var rows = await conn.QueryAsync<Book>(cmd);
        return rows.ToList();
    }

    public async Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var conn = await _dataSource.OpenConnectionAsync(cancellationToken);
        var cmd = new CommandDefinition(
            "SELECT id, title, author FROM books WHERE id = @Id",
            new { Id = id },
            cancellationToken: cancellationToken);
        return await conn.QuerySingleOrDefaultAsync<Book>(cmd);
    }

    public async Task<Book> CreateAsync(string title, string author, CancellationToken cancellationToken = default)
    {
        await using var conn = await _dataSource.OpenConnectionAsync(cancellationToken);
        var cmd = new CommandDefinition(
            """
            INSERT INTO books (title, author)
            VALUES (@Title, @Author)
            RETURNING id, title, author;
            """,
            new { Title = title, Author = author },
            cancellationToken: cancellationToken);
        return await conn.QuerySingleAsync<Book>(cmd);
    }

    public async Task<Book?> UpdateAsync(int id, string title, string author, CancellationToken cancellationToken = default)
    {
        await using var conn = await _dataSource.OpenConnectionAsync(cancellationToken);
        var cmd = new CommandDefinition(
            """
            UPDATE books
            SET title = @Title, author = @Author
            WHERE id = @Id
            RETURNING id, title, author;
            """,
            new { Id = id, Title = title, Author = author },
            cancellationToken: cancellationToken);
        return await conn.QuerySingleOrDefaultAsync<Book>(cmd);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var conn = await _dataSource.OpenConnectionAsync(cancellationToken);
        var cmd = new CommandDefinition(
            "DELETE FROM books WHERE id = @Id",
            new { Id = id },
            cancellationToken: cancellationToken);
        var affected = await conn.ExecuteAsync(cmd);
        return affected > 0;
    }
}
