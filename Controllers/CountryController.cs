using CountryAPI.Attributes;
using CountryAPI.Models.Country;
using CountryAPI.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;


namespace CountryAPI.Controllers
{
    [BearerAuthorize]
    [RoleAuthorization("Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        /// <summary>
        /// List of All Countries With Specific Language
        /// </summary>
        /// <param name="language"></param>
        /// <returns>Return ISO3166 Countries</returns>
        [HttpGet("GetAllCountries")]
        public async Task<IActionResult> GetAllCountry([FromQuery] string language)
        {
            using var client = new HttpClient();
            const string url = "https://raw.githubusercontent.com/dr5hn/countries-states-cities-database/master/countries%2Bstates%2Bcities.json";

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var json = JArray.Parse(responseBody);
            var languageList = json.Select(data => (string)data["translations"][$"{language}"]);


            if (language == "en")
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
                    Countries = json.Select(data => (string)data["translations"][$"{language}"]).ToList()
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
        /// <param name="countryName"></param>
        /// <returns>Return List States</returns>
        [HttpGet("GetStateOfCountry")]
        public async Task<IActionResult> GetStatesOfCountry([FromQuery] string countryName)
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
                return NotFound(new ErrorResponse("Country Not Found"));
            }
            return Ok(states);
        }

        /// <summary>
        /// List Of Cities
        /// </summary>
        /// <param name="countryName"></param>
        /// <param name="statesName"></param>
        /// <returns>Return List Cities</returns>
        [HttpGet("GetCitiesOfStates")]
        public async Task<IActionResult> GetCitiesOfStates([FromQuery] string countryName, string statesName)
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
                .Where(obj => (string)obj["name"] == statesName)
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
        /// <param name="countryName"></param>
        /// <returns>Return List Currency Code, Name and Symbol</returns>
        [HttpGet("GetCurrencyOfCountry")]
        public async Task<IActionResult> GetCurrencyOfCountry([FromQuery] string countryName)
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

