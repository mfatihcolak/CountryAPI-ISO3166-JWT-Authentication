using CountryAPI.Models.Identity;
using CountryAPI.Models.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CountryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(
            UserManager<IdentityUser> userManager
        )
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Get User By UserName
        /// </summary>
        /// <param name="username"></param>
        /// <returns>User</returns>
        [HttpGet("Username")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IdentityUser))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetUser([FromQuery] string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user is null)
            {
                return NotFound(new ErrorResponse("User not found"));
            }
            return Ok(user);
        }

        /// <summary>
        /// Update User
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Return Updated User</returns>        
        [HttpPut("UpdateUser")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IdentityResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
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
        /// Delete User
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>Return Response Message</returns>
        [HttpDelete("DeleteUser")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> DeleteUser([FromQuery] string userName)
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
