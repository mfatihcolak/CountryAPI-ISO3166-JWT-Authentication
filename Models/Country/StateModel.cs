using System.Runtime.Serialization;

namespace CountryAPI.Models.Country
{
    public class StateModel
    {
        [DataMember(Name = "name")]
        public string? Name { get; set; }
        [DataMember(Name = "code")]
        public string? Code { get; set; }
    }
}
