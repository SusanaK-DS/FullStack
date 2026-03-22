using Backend.Data;
using Dapper;
using Microsoft.OpenApi.Models;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("Connection string 'DefaultConnection' is missing. Set it in appsettings.json or environment variable ConnectionStrings__DefaultConnection.");

builder.Services.AddSingleton<NpgsqlDataSource>(_ => new NpgsqlDataSourceBuilder(connectionString).Build());
builder.Services.AddScoped<IBookRepository, BookRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Library API",
        Version = "v1",
        Description = "Books CRUD and health endpoints"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Library API v1");
        options.RoutePrefix = "swagger";
    });

    try
    {
        var dataSource = app.Services.GetRequiredService<NpgsqlDataSource>();
        await using var conn = await dataSource.OpenConnectionAsync();
        await conn.ExecuteAsync("""
            CREATE TABLE IF NOT EXISTS Books (
                Id SERIAL PRIMARY KEY,
                Title CHARACTER VARYING(500) NOT NULL,
                Author CHARACTER VARYING(200) NOT NULL
            );
            """);
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
        logger.LogWarning(ex, "Could not connect to the database during startup; ensure PostgreSQL is running. The API will still start.");
    }
}

// In Development, skip redirect so HTTP (e.g. curl http://localhost:5259) is not sent to HTTPS with a dev certificate.
if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.MapGet("/api/health/db", async (NpgsqlDataSource dataSource) =>
{
    try
    {
        await using var conn = await dataSource.OpenConnectionAsync();
        await conn.ExecuteScalarAsync<int>("SELECT 1");
        return Results.Ok(new { connected = true, message = "Database connection succeeded." });
    }
    catch (Exception ex)
    {
        return Results.Json(
            new { connected = false, message = "Database connection failed.", error = ex.Message },
            statusCode: StatusCodes.Status503ServiceUnavailable);
    }
})
.WithName("CheckDatabaseConnection");

app.MapControllers();

app.Run();
