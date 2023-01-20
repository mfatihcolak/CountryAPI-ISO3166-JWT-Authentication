using Newtonsoft.Json;

namespace CountryAPI.Models.ResponseModels
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public int StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public string Error { get; set; }

        public static ApiResponse<T> Success(T data, int statusCode)
        {
            return new ApiResponse<T> { Data = data, StatusCode = statusCode, IsSuccess = true };

        }

        public static ApiResponse<T> Fail(string error, int statusCode)
        {
            return new ApiResponse<T> { Error = error, StatusCode = statusCode, IsSuccess = false };
        }
        
    }

}
