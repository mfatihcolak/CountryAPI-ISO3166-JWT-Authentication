using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace CountryAPI.Models
{
    /// <summary>
    /// Role Assign
    /// </summary>
    [DataContract]
    public class AssignRoleModel
    {
        /// <summary>
        /// User ID
        /// </summary>
        [Required]
        [DataMember(Name = "userId")]
        public string UserId { get; set; } = default!;

        /// <summary>
        /// Role Name
        /// </summary>
        [Required]
        [DataMember(Name = "roleName")]
        public string RoleName { get; set; } = default!;
    }
}
