using Microsoft.AspNetCore.Mvc;

namespace CountryAPI.Error
{
    public class CountryCustomDetails : ProblemDetails
    {
        public string? AdditionalInfo { get; set; }
    }
}
