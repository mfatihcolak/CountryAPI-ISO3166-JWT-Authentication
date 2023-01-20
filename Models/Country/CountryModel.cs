using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections;

namespace CountryAPI.Models.Country
{
    /// <summary>
    /// Country Model
    /// </summary>
    public class CountryModel
    {
        /// <summary>
        /// All Country
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// ISO2 Code
        /// </summary>
        public string? Iso2 { get; set; }
        /// <summary>
        /// ISO3 Code
        /// </summary>
        public string? Iso3 { get; set; }
        /// <summary>
        /// Capital
        /// </summary>
        public string? Capital { get; set; }
    }
}
