namespace Guess_Word_Backend.Models
{
    public class ApiResponse<T> where T : class
    {
        public ApiResponse()
        {
        }

        public bool Success { get; set; }
        public T? Data { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;

        public ApiResponse(bool success, T? data, int statusCode, string message)
        {
            Success = success;
            Data = data;
            StatusCode = statusCode;
            Message = message;
        }

        public static ApiResponse<T> Ok(T? data = null)
        {
            return new ApiResponse<T>(true,data, 200, "OK");
        }
        public static ApiResponse<T> Created(T? data = null, string? message = default)
        {
            return new ApiResponse<T>(true,data, 201, message??"Created");
        }
        public static ApiResponse<T> Updated(T? data = null, string? message = default)
        {
            return new ApiResponse<T>(true,data, 200, "Updated");
        }
        public static ApiResponse<T> Deleted(T? data = null, string? message = default)
        {
            return new ApiResponse<T>(true,data, 200, "Deleted");
        }
        public static ApiResponse<T> BadRequest(T? data = null, string? message = default)
        {
            return new ApiResponse<T>(false,data, 400, message);
        }
       
    }
}
