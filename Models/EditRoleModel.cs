using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace CountryAPI.Models
{
    /// <summary>
    /// Edit User Role
    /// </summary>
    [DataContract]
    public class EditRoleModel
    {
        /// <summary>
        /// Role Id
        /// </summary>
        [Required]
        [DataMember(Name = "RoleId")]
        public string RoleId { get; set; } = string.Empty;

        /// <summary>
        /// User Id
        /// </summary>
        [Required]
        [DataMember(Name = "UserId")]
        public string UserId { get; set; } = string.Empty;
    }
}
