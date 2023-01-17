using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace CountryAPI.Models
{
    /// <summary>
    /// Role Model
    /// </summary>
    [DataContract]
    public class RoleModel
    {
        /// <summary>
        /// Role Name
        /// </summary>
        [Required]
        [DataMember(Name = "RoleName")]
        public string RoleName { get; set; } = string.Empty;
    }
}
