using System.Net;

namespace ShopApi.Model;

public class ApiResponse
{
    public ApiResponse()
    {
        ErrorMessages = new List<string>();
    }
    public HttpStatusCode  StatusCode { get; set; }
    public bool IsSuccess { get; set; } = true;
    public int Total { get; set; }
    public List<string> ErrorMessages { get; set; }
    public object Result { get; set; }
}