using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using CountryAPI.Models.Identity;
using CountryAPI.Models;
using CountryAPI.Models.ResponseModels;

namespace CountryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthController(
            IConfiguration configuration,
            RoleManager<AppRole> roleManager,
            UserManager<IdentityUser> userManager
        )
        {
            _configuration = configuration;
            _roleManager = roleManager;
            _userManager = userManager;
        }



        [HttpGet]
        public async Task<ActionResult<string>> TestMe([FromQuery] string username)
        {
            var userName = await _userManager.FindByNameAsync(username);
            return Ok(userName);
        }

        /// <summary>
        /// User Registration
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Returns Response</returns>
        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] AppUser user)
        {
            var dbUser = await _userManager.FindByNameAsync(user.UserName);
            if (dbUser is not null)
                return Conflict(new ErrorResponse("User already exists!"));

            dbUser = new IdentityUser { UserName = user.UserName, Email = user.Email };
            await _userManager.CreateAsync(dbUser, user.Password);

            var role = await _roleManager.FindByNameAsync("User");
            if (role is null)
                await _roleManager.CreateAsync(new AppRole { Name = "User" });

            await _userManager.AddToRoleAsync(dbUser, "User");

            return Ok(new SuccessResponse("User created successfully!"));
        }

        /// <summary>
        /// User Login
        /// </summary>
        /// <param name="Username">User Name</param>
        /// <param name="Password">User Password</param>
        /// <returns>Return Jwt Token </returns>
        /// <exception cref="Exception">Could not find the secret in config</exception>
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return Forbid();

            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            authClaims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));

            var secret = _configuration["JWT:Secret"];

            if (secret is null)
                throw new Exception("Could not find the secret in config");

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });

        }
    }
}
