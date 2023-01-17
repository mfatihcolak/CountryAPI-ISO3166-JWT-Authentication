using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace CountryAPI.Models
{
    /// <summary>
    /// Login Model
    /// </summary>
    [DataContract]
    public class LoginModel
    {
        /// <summary>
        /// Username
        /// </summary>
        [Required]
        [DataMember(Name = "UserName")]
        public string Username { get; set; } = default!;
        /// <summary>
        /// Password
        /// </summary>
        [Required]
        [DataMember(Name = "Password")]
        public string Password { get; set; } = default!;
    }
}
