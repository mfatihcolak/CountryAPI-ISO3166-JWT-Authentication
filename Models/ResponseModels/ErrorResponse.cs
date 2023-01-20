namespace CountryAPI.Models.ResponseModels
{
    public class ErrorResponse : BaseResponseModel
    {
        /// <summary>
        /// Error Response
        /// </summary>
        public ErrorResponse(string message) : base("Error", message)
        {

        }
    }
}
