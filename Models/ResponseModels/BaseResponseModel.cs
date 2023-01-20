namespace CountryAPI.Models.ResponseModels
{
    /// <summary>
    /// Response Model Base
    /// </summary>
    public class BaseResponseModel
    {
        /// <summary>
        /// Base Response Model
        /// </summary>
        public BaseResponseModel(string status, string message)
        {
            Status = status;
            Message = message;
        }

        public string Status { get; set; }
        public string Message { get; set; }
    }
}
