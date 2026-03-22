using System.Net;

namespace Backend.Models;

public class BaseResponse
{
    public HttpStatusCode HttpStatus { get; set; } = HttpStatusCode.OK;
    public string? ErrorMessage { get; set; }
    public int ErrorCode { get; set; } = 1;
    public bool Status { get; set; } = true;
    public object? Data { get; set; }
}
