using CountryAPI.Attributes;
using CountryAPI.Models.Country;
using CountryAPI.Models.ResponseModels;
using CountryAPI.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Cryptography.X509Certificates;

namespace CountryAPI.Controllers
{
    /// <summary>
    /// Everything about country
    /// </summary>
    //[BearerAuthorize]
    //[RoleAuthorization("Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : CustomBaseController
    {
        private readonly CountryService _countryService;
        private readonly ProblemDetailsFactory _problemDetailsFactory;

        /// <summary>
        /// Constructor 
        /// </summary>
        public CountryController(CountryService countryService, ProblemDetailsFactory problemDetailsFactory)
        {
            _countryService = countryService;
            _problemDetailsFactory = problemDetailsFactory;
        }

        /// <summary>
        /// Get All Countries 
        /// </summary>
        /// <param name="languageCode">Language code to get all countries</param>
        /// <response code="200">successful operation</response>
        /// <response code="404">Not supported language</response>
        [HttpGet]
        [Route("countries/{languageCode}")]
        [SwaggerOperation("GetAllCountry")]
        [ValidateModelState]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<string>>), Description = "Successful operation")]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails), Description = "Not Supported Language")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails), Description = "Invalid Code")]
        public async Task<IActionResult> GetAllCountry([FromRoute] string languageCode)
        {
            var json = await _countryService.GetCountryData();
            if (languageCode == "en")
            {
                var countries = json.Select(item => new CountryModel
                {
                    Iso3 = (string?)item["iso3"],
                    Iso2 = (string?)item["iso2"],
                    Name = (string?)item["name"],
                    Capital = (string?)item["capital"]
                }).ToList();
                return Ok(ApiResponse<List<CountryModel>>.Success(countries, 200));
            }
            else
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "Not Supported Language",
                    Detail = $"The language '{languageCode}' is not supported.",
                    Status = StatusCodes.Status404NotFound,
                    Type = "/errors/incorrect-languageCode",
                    Instance = "/api/Country/countries/en"
                };
                return NotFound(problemDetails);
            }
            
        }

        /// <summary>
        /// Get States
        /// </summary>
        /// <param name="countryCode">Country ISO3166 Two Letter Code </param>
        /// <response code="200">successful operation</response>
        /// <response code="404">Country name not found</response>
        [HttpGet]
        [Route("states/{countryCode}")]
        [SwaggerOperation("GetStatesOfCountry")]
        [ValidateModelState]
        [SwaggerResponse(statusCode: 200,type: typeof(StateModel), description: "Successful operation")]
        [SwaggerResponse(statusCode: 404,type: typeof(ErrorResponse), description: "Not Found")]
        public async Task<ApiResponse<IEnumerable<StateModel>>> GetStatesOfCountry([FromRoute] string countryCode)
        {
            var json = await _countryService.GetCountryData();

            var states = json.Where(obj => (string)obj["iso2"] == countryCode)
                .Where(obj => obj["states"] != null)
                .SelectMany(obj => (JArray)obj["states"])
                .Where(states => states["name"] != null && states["state_code"] != null)
                .Select(states => new StateModel { Name = (string)states["name"], Code = (string)states["state_code"] });


            if (!states.Any())
            {
                return ApiResponse<IEnumerable<StateModel>>.Fail("Country Name Not Found", 404);
            }
            return ApiResponse<IEnumerable<StateModel>>.Success(states, 200);
        }

        /// <summary>
        /// Get Cities
        /// </summary>
        /// <param name="countryName">Country ISO3166 Two Letter Code</param>
        /// <param name="stateName">State Code</param>
        /// <returns>Return List Cities</returns>
        /// <response code="200">successful operation</response>
        /// <response code="404">Country name not found</response>
        [HttpGet]
        [Route("cities/{countryCode}/{stateCode}")]
        [SwaggerOperation("GetAllCountry")]
        [ValidateModelState]
        [SwaggerResponse(statusCode: 200, type: typeof(CityModel), description: "Successful operation")]
        [SwaggerResponse(statusCode: 404, type: typeof(ErrorResponse), description: "Notfound")]
        public async Task<ApiResponse<CityModel>> GetCitiesOfStates([FromRoute] string countryCode, string stateCode)
        {
            var json = await _countryService.GetCountryData();

            var cities = json.Where(obj => (string)obj["iso2"] == countryCode)
                .Where(obj => obj["states"] != null)
                .SelectMany(obj => (JArray)obj["states"])
                .Where(obj => (string)obj["state_code"] == stateCode)
                .Where(obj => obj["cities"] != null)
                .SelectMany(obj => (JArray)obj["cities"])
                .Where(obj => obj["name"] != null)
                .Select(obj => (string)obj["name"]);

            if (!cities.Any())
            {
                return ApiResponse<CityModel>.Fail(stateCode + "NOT FOUND", 404);
            }
            var cityEntry = new CityModel
            {
                Cities = cities
            };
            return ApiResponse<CityModel>.Success(cityEntry, 200);
        }

        /// <summary>
        /// Get Currency
        /// </summary>
        /// <param name="countryCode">Country ISO3166 Two Letter Code</param>
        /// <returns>Return List Currency Code, Name and Symbol</returns>
        /// <response code="200">successful operation</response>
        /// <response code="404">Country name not found</response>
        [HttpGet]
        [Route("currency/{countryCode}")]
        [SwaggerOperation("GetAllCountry")]
        [ValidateModelState]
        [SwaggerResponse(statusCode: 200, type: typeof(CurrencyModel), description: "Successful operation")]
        [SwaggerResponse(statusCode: 404, type: typeof(ErrorResponse), description: "Notfound")]
        public async Task<ApiResponse<CurrencyModel>> GetCurrencyOfCountry([FromRoute] string countryCode)
        {
            var json = await _countryService.GetCountryData();

            var city = json.FirstOrDefault(obj => (string?)obj["iso2"] == countryCode);

            if (city is null)
            {
                return ApiResponse<CurrencyModel>.Fail(countryCode + " Not Found", 404);
            }

            var currency = (string?)city["currency"];
            var currencyName = (string?)city["currency_name"];
            var currencySymbol = (string?)city["currency_symbol"];


            var currencyEntry = new CurrencyModel
            {
                Name = currencyName,
                Code = currency,
                Symbol = currencySymbol

            };
            return ApiResponse<CurrencyModel>.Success(currencyEntry, 200);
        }
    }
}

