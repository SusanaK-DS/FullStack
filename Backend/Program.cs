using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("Connection string 'DefaultConnection' is missing. Set it in appsettings.json or environment variable ConnectionStrings__DefaultConnection.");

builder.Services.AddOpenApi();
builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<LibraryContext>();
        await db.Database.MigrateAsync();
    }
}

app.UseHttpsRedirection();

app.MapGet("/api/books", async (LibraryContext db) =>
    await db.Books.AsNoTracking().OrderBy(b => b.Id).ToListAsync())
.WithName("GetBooks");

app.MapGet("/api/books/{id:int}", async (int id, LibraryContext db) =>
    await db.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id) is { } book
        ? Results.Ok(book)
        : Results.NotFound())
.WithName("GetBookById");

app.MapPost("/api/books", async (CreateBookDto dto, LibraryContext db) =>
{
    if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Author))
        return Results.BadRequest(new { error = "Title and Author are required." });

    var book = new Book { Title = dto.Title.Trim(), Author = dto.Author.Trim() };
    db.Books.Add(book);
    await db.SaveChangesAsync();
    return Results.Created($"/api/books/{book.Id}", book);
})
.WithName("CreateBook");

app.Run();

record CreateBookDto(string Title, string Author);
