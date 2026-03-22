namespace Backend.Models;

public record CreateBookRequest(string Title, string Author);

public record UpdateBookRequest(string Title, string Author);
