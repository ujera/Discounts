namespace Discounts.Application.Exceptions
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        // Success
        public ApiResponse(T data, string message = "")
        {
            Success = true;
            Message = message;
            Data = data;
            Errors = null;
        }

        // Error
        public ApiResponse(string message, List<string>? errors = null)
        {
            Success = false;
            Message = message;
            Data = default;
            Errors = errors;
        }
    }
}
