using System.Runtime.Serialization;
using System.Xml.Linq;

namespace CountryAPI.Models.Country
{
    public class CurrencyModel
    {
        [DataMember(Name = "code")]
        public string Code { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "symbol")]
        public string Symbol { get; set; }
    }
}
