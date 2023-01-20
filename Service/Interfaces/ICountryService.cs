using CountryAPI.Models.Country;
using CountryAPI.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace CountryAPI.Service.Interfaces
{
    public interface ICountryService
    {
        public Task<JArray> GetCountryData();       
    }
}
