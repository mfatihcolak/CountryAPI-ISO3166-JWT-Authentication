using Microsoft.AspNetCore.Identity;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace CountryAPI.Models.Identity
{
    /// <summary>
    /// User Register
    /// </summary>
    [DataContract]
    public class AppUser
    {
        /// <summary>
        /// User Name
        /// </summary>
        [DataMember(Name = "userName")]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// User Password
        /// </summary>
        [DataMember(Name = "password")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// User Email
        /// </summary>
        [DataMember(Name = "email")]
        public string Email { get; set; } = string.Empty;

        public static AppUser From(IdentityUser identityUser)
        {
            var appUser = new AppUser
            {
                UserName = identityUser.UserName,
                Email = identityUser.Email
            };

            return appUser;
        }
    }
}

