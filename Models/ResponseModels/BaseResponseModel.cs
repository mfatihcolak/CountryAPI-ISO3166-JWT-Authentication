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
        /// <param name="status">Response</param>
        /// <param name="message">Message</param>
        public BaseResponseModel(string status, string message)
        {
            Status = status;
            Message = message;
        }

        public string Status { get; set; }
        public string Message { get; set; }
    }
}
