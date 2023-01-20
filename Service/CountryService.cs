using CountryAPI.Controllers;
using CountryAPI.Models.Country;
using CountryAPI.Models.ResponseModels;
using CountryAPI.Service.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net;

namespace CountryAPI.Service
{
    public class CountryService : ICountryService
    {
        private readonly HttpClient _httpclient;
        private const string BASE_URL = "https://raw.githubusercontent.com/dr5hn/countries-states-cities-database/master/countries%2Bstates%2Bcities.json";

        /// <summary>
        /// Constructor
        /// </summary>
        public CountryService(HttpClient httpClient)
        {
            _httpclient = httpClient;
        }

        /// <summary>
        /// Get Data and parsing
        /// </summary>
        /// <returns></returns>
        public async Task<JArray> GetCountryData()
        {
            var response = await _httpclient.GetAsync(BASE_URL);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            return JArray.Parse(responseBody);
        }

        
    }
}
