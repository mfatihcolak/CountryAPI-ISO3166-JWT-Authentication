using CountryAPI.Attributes;
using CountryAPI.Models.Country;
using CountryAPI.Models.Identity;
using CountryAPI.Models.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CountryAPI.Controllers
{
    /// <summary>
    /// Everything about user
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userManager"></param>
        public UserController(
            UserManager<IdentityUser> userManager
        )
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Get user by user name
        /// </summary>
        /// <param name="userName">Language code to get all countries</param>
        /// <response code="200">successful operation</response>
        /// <response code="404">User not found</response>
        [HttpGet]
        [Route("get/{userName}")]
        [SwaggerOperation("GetUser")]
        [ValidateModelState]
        [SwaggerResponse(statusCode: 200, type: typeof(IdentityUser), description: "Successful operation")]
        [SwaggerResponse(statusCode: 404, type: typeof(ErrorResponse), description: "User not found")]
        public async Task<IActionResult> GetUser([FromRoute] string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user is null)
            {
                return NotFound(new ErrorResponse("User not found"));
            }
            return Ok(user);
        }

        /// <summary>
        /// Update User
        /// </summary>
        /// <param name="user">App User Model</param>
        /// <response code="200">successful operation</response>
        /// <response code="404">Not supported language</response>
        [HttpPut]
        [Route("put")]
        [SwaggerOperation("UpdateUser")]
        [ValidateModelState]
        [SwaggerResponse(statusCode: 200, type: typeof(IdentityUser), description: "Successful operation")]
        [SwaggerResponse(statusCode: 404, type: typeof(ErrorResponse), description: "User not found")]
        public async Task<IActionResult> UpdateUser([FromBody] AppUser user)
        {
            var dbUser = await _userManager.FindByNameAsync(user.UserName);
            if (dbUser is null)
            {
                return NotFound(new ErrorResponse("User not found"));
            }
            dbUser.Email = user.Email;
            var result = await _userManager.UpdateAsync(dbUser);

            return Ok(result);
        }

        /// <summary>
        /// Delete user by user name
        /// </summary>
        /// <param name="userName">User name</param>
        /// <response code="200">User Deleted Successfully!</response>
        /// <response code="404">User not found</response>
        [HttpDelete]
        [Route("delete/{userName}")]
        [SwaggerOperation("GetAllCountry")]
        [ValidateModelState]
        [SwaggerResponse(statusCode: 200, type: typeof(SuccessResponse), description: "User Deleted Successfully!")]
        [SwaggerResponse(statusCode: 404, type: typeof(ErrorResponse), description: "User not found")]
        public async Task<IActionResult> DeleteUser([FromRoute] string userName)
        {
            var userCheck = await _userManager.FindByNameAsync(userName);
            if (userCheck is null)
            {
                return NotFound(new ErrorResponse("User not found"));
            }

            await _userManager.DeleteAsync(userCheck);
            return Ok(new SuccessResponse("User Deleted Successfully!"));
        }
    }
}
