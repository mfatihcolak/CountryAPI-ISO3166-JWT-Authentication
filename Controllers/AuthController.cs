using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using CountryAPI.Models.Identity;
using CountryAPI.Models;
using CountryAPI.Models.ResponseModels;
using CountryAPI.Attributes;
using CountryAPI.Models.Country;
using Swashbuckle.AspNetCore.Annotations;

namespace CountryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="roleManager"></param>
        /// <param name="userManager"></param>
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

        /// <summary>
        /// Register
        /// </summary>
        /// <param name="user">Language code to get all countries</param>
        /// <response code="200">successful operation</response>
        /// <response code="404">Not supported language</response>
        [HttpPost]
        [Route("register")]
        [SwaggerOperation("Register")]
        [ValidateModelState]
        [SwaggerResponse(statusCode: 200, type: typeof(SuccessResponse), description: "User created successfully!")]
        [SwaggerResponse(statusCode: 409, type: typeof(ErrorResponse), description: "User already exists!")]
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
        /// Login
        /// </summary>
        /// <param name="model">Login Model</param>
        /// <response code="200">successful operation</response>
        /// <response code="403">Forbidden</response>
        [HttpPost]
        [Route("login")]
        [SwaggerOperation("Login")]
        [ValidateModelState]
        [SwaggerResponse(statusCode: 200)]
        [SwaggerResponse(statusCode: 403)]
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
