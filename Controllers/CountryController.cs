using CountryAPI.Attributes;
using CountryAPI.Models.Country;
using CountryAPI.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Annotations;

namespace CountryAPI.Controllers
{
    /// <summary>
    /// Everything about country
    /// </summary>
    //[BearerAuthorize]
    //[RoleAuthorization("Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        /// <summary>
        /// List of All Countries With Specific Language
        /// </summary>
        /// <param name="languageCode">Language code to get all countries</param>
        /// <response code="200">successful operation</response>
        /// <response code="404">Not supported language</response>
        [HttpGet("countries/{languageCode}")]
        //[Route("countries/{languageCode}")]
        [SwaggerOperation("GetAllCountry")]
        [ValidateModelState]
        [SwaggerResponse(statusCode: 200,type: typeof(CountryModel), description: "Successful operation")]
        [SwaggerResponse(statusCode: 404,type: typeof(ErrorResponse), description: "Notfound")]
        public async Task<IActionResult> GetAllCountry([FromRoute] string languageCode)
        {
            using var client = new HttpClient();
            const string url = "https://raw.githubusercontent.com/dr5hn/countries-states-cities-database/master/countries%2Bstates%2Bcities.json";

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var json = JArray.Parse(responseBody);
            var languageList = json.Select(data => (string)data["translations"][$"{languageCode}"]);


            if (languageCode == "en")
            {
                var countriesList = new CountryModel
                {
                    Countries = json.Select(data => (string)data["name"]).ToList()
                };
                return Ok(countriesList);
            }
            else if (languageList.Any(x => x != null))
            {
                var countriesList = new CountryModel
                {
                    Countries = json.Select(data => (string)data["translations"][$"{languageCode}"]).ToList()
                };
                return Ok(countriesList);
            }
            else
            {
                return NotFound(new ErrorResponse("Not Supported Language"));
            }
        }

        /// <summary>
        /// Get List States
        /// </summary>
        /// <param name="countryName">Country name to get states of country</param>
        /// <response code="200">successful operation</response>
        /// <response code="404">Country name not found</response>
        [HttpGet]
        [Route("states/{countryName}")]
        [SwaggerOperation("GetStatesOfCountry")]
        [ValidateModelState]
        [SwaggerResponse(statusCode: 200,type: typeof(StateModel), description: "Successful operation")]
        [SwaggerResponse(statusCode: 404,type: typeof(ErrorResponse), description: "Not Found")]
        public async Task<IActionResult> GetStatesOfCountry([FromRoute] string countryName)
        {
            using var client = new HttpClient();
            var url = "https://raw.githubusercontent.com/dr5hn/countries-states-cities-database/master/countries%2Bstates%2Bcities.json";

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var json = JArray.Parse(responseBody);

            var states = json.Where(obj => (string)obj["name"] == countryName)
                .Where(obj => obj["states"] != null)
                .SelectMany(obj => (JArray)obj["states"])
                .Where(states => states["name"] != null && states["state_code"] != null)
                .Select(states => new StateModel { Name = (string)states["name"], Code = (string)states["state_code"] });


            if (!states.Any())
            {
                return NotFound(new ErrorResponse("Country Name Not Found"));
            }
            return Ok(states);
        }

        /// <summary>
        /// List Of Cities
        /// </summary>
        /// <param name="countryName">Country name to get citites of states</param>
        /// <param name="statesName">States name to get citites of states</param>
        /// <returns>Return List Cities</returns>
        /// <response code="200">successful operation</response>
        /// <response code="404">Country name not found</response>
        [HttpGet]
        [Route("cities/{countryName}/{stateName}")]
        [SwaggerOperation("GetAllCountry")]
        [ValidateModelState]
        [SwaggerResponse(statusCode: 200, type: typeof(CityModel), description: "Successful operation")]
        [SwaggerResponse(statusCode: 404, type: typeof(ErrorResponse), description: "Notfound")]
        public async Task<IActionResult> GetCitiesOfStates([FromRoute] string countryName, string stateName)
        {
            using var client = new HttpClient();
            var url = "https://raw.githubusercontent.com/dr5hn/countries-states-cities-database/master/countries%2Bstates%2Bcities.json";

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var json = JArray.Parse(responseBody);

            var cities = json.Where(obj => (string)obj["name"] == countryName)
                .Where(obj => obj["states"] != null)
                .SelectMany(obj => (JArray)obj["states"])
                .Where(obj => (string)obj["name"] == stateName)
                .Where(obj => obj["cities"] != null)
                .SelectMany(obj => (JArray)obj["cities"])
                .Where(obj => obj["name"] != null)
                .Select(obj => (string)obj["name"]);

            if (!cities.Any())
            {
                return NotFound(new ErrorResponse("Country Not Found"));
            }
            var cityEntry = new CityModel
            {
                Cities = cities
            };
            return Ok(cityEntry);
        }

        /// <summary>
        /// Currency
        /// </summary>
        /// <param name="countryName">Country name to get currency information of country</param>
        /// <returns>Return List Currency Code, Name and Symbol</returns>
        /// <response code="200">successful operation</response>
        /// <response code="404">Country name not found</response>
        [HttpGet]
        [Route("currency/{countryName}")]
        [SwaggerOperation("GetAllCountry")]
        [ValidateModelState]
        [SwaggerResponse(statusCode: 200, type: typeof(CurrencyModel), description: "Successful operation")]
        [SwaggerResponse(statusCode: 404, type: typeof(ErrorResponse), description: "Notfound")]
        public async Task<IActionResult> GetCurrencyOfCountry([FromRoute] string countryName)
        {
            using var client = new HttpClient();
            var url = "https://raw.githubusercontent.com/dr5hn/countries-states-cities-database/master/countries%2Bstates%2Bcities.json";

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var json = JArray.Parse(responseBody);

            var city = json.FirstOrDefault(obj => (string)obj["name"] == countryName);

            if (city is null)
            {
                return NotFound(new ErrorResponse("Country Not Found"));
            }

            var currency = (string)city["currency"];
            var currencyName = (string)city["currency_name"];
            var currencySymbol = (string)city["currency_symbol"];


            var currencyEntry = new CurrencyModel
            {
                Name = currencyName,
                Code = currency,
                Symbol = currencySymbol

            };
            return Ok(currencyEntry);
        }
    }
}

