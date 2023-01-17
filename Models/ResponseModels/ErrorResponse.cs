namespace CountryAPI.Models.ResponseModels
{
    public class ErrorResponse : BaseResponseModel
    {
        /// <summary>
        /// Error Response
        /// </summary>
        /// <param name="message"></param>
        public ErrorResponse(string message) : base("Error", message)
        {

        }
    }
}
