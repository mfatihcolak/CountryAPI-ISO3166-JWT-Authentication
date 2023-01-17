using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CountryAPI.Models.ResponseModels;
using CountryAPI.Models;
using CountryAPI.Models.Identity;

namespace CountryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public RoleController(
            RoleManager<AppRole> roleManager,
            UserManager<IdentityUser> userManager
        )
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        /// <summary>
        /// Create Role
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Return created role</returns>
        [HttpPost("CreateRole")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SuccessResponse))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> CreateRole([FromBody] RoleModel model)
        {
            var roleExist = await _roleManager.FindByNameAsync(model.RoleName);
            if (roleExist != null)
            {
                return Conflict(new ErrorResponse("Role already exist"));
            }
            await _roleManager.CreateAsync(new AppRole { Name = model.RoleName });

            return Ok(new SuccessResponse("Successfully created role"));
        }

        /// <summary>
        /// Role Assign
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Return Response Message</returns>
        [HttpPost("AssignRoleToUser")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> AddRoleToUser([FromBody] AssignRoleModel model)
        {
            var role = await _roleManager.FindByNameAsync(model.RoleName);
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user is null)
            {
                return NotFound(new ErrorResponse("User Not found"));
            }
            if (role is null)
            {
                return NotFound(new ErrorResponse("Role Not Found"));
            }

            await _userManager.AddToRoleAsync(user, model.RoleName);
            return Ok(new SuccessResponse("Successfully added to the role"));
        }

        /// <summary>
        /// Get User Role By User Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Return Assigned Roles</returns>
        [HttpGet("GetUserRolesByUserId")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetUserRolesByUserId([FromBody] string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return NotFound(new ErrorResponse("User Not found"));
            }
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles is null)
            {
                return NotFound(new ErrorResponse("Roles Not Found"));
            }
            return Ok(new { Roles = userRoles });
        }

        /// <summary>
        /// Delete Existing Role
        /// </summary>
        /// <param name="RoleName"></param>
        /// <returns>Return Response Message</returns>        
        [HttpDelete("DeleteRole")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> DeleteRole([FromQuery] RoleModel model)
        {
            var role = await _roleManager.FindByNameAsync(model.RoleName);
            if (role is null)
            {
                return NotFound(new ErrorResponse("Role Does Not Exist"));
            }

            await _roleManager.DeleteAsync(role);
            return Ok(new SuccessResponse("Successfully deleted the role"));
        }
    }
}
