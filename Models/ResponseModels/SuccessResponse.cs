namespace CountryAPI.Models.ResponseModels
{
    public class SuccessResponse : BaseResponseModel
    {
        /// <summary>
        /// Succes Response
        /// </summary>
        /// <param name="message"></param>
        public SuccessResponse(string message) : base("Success", message)
        {

        }
    }
}
