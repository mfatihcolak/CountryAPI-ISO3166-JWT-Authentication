using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CountryAPI.Models.ResponseModels;
using CountryAPI.Models;
using CountryAPI.Models.Identity;
using CountryAPI.Attributes;
using CountryAPI.Models.Country;
using Swashbuckle.AspNetCore.Annotations;

namespace CountryAPI.Controllers
{
    /// <summary>
    /// Everything about role 
    /// </summary>
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
        /// Create role
        /// </summary>
        /// <param name="model">Role Model</param>
        /// <response code="200">successful operation</response>
        /// <response code="404">Country name not found</response>
        [HttpPost]
        [Route("create")]
        [SwaggerOperation("CreateRole")]
        [ValidateModelState]
        [SwaggerResponse(statusCode: 200, type: typeof(SuccessResponse), description: "Successful operation")]
        [SwaggerResponse(statusCode: 409, type: typeof(ErrorResponse), description: "Already exist")]
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
        /// Add role to user
        /// </summary>
        /// <param name="model">Assign Role Model</param>
        /// <response code="200">successful operation</response>
        /// <response code="404">Not supported language</response>
        [HttpPost]
        [Route("add")]
        [SwaggerOperation("AddRoleToUser")]
        [ValidateModelState]
        [SwaggerResponse(statusCode: 200, type: typeof(SuccessResponse), description: "Successful operation")]
        [SwaggerResponse(statusCode: 404, type: typeof(ErrorResponse), description: "Notfound")]
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
        /// Get user roles by user Id
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <response code="200">successful operation</response>
        /// <response code="404">Roles Not Found</response>
        [HttpGet]
        [Route("user/{userId}")]
        [SwaggerOperation("GetUserRolesByUserId")]
        [ValidateModelState]
        [SwaggerResponse(statusCode: 200, description: "Successful operation")]
        [SwaggerResponse(statusCode: 404, type: typeof(ErrorResponse), description: "Roles Not Found")]
        public async Task<IActionResult> GetUserRolesByUserId([FromRoute] string userId)
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
            var response = new { Roles = userRoles };
            return Ok(response);
        }

        /// <summary>
        /// Delete role by role name
        /// </summary>
        /// <param name="roleName">Delete role with role name</param>
        /// <response code="200">Successfully deleted the role</response>
        /// <response code="404">Role Does Not Exist</response>
        /// <response code="default">Problem Details (RFC 7807) Response.</response>
        [HttpDelete]
        [Route("delete/{roleName}")]
        [SwaggerOperation("DeleteRole")]
        [ValidateModelState]
        [SwaggerResponse(statusCode: 200, type: typeof(SuccessResponse), description: "Successfully deleted the role")]
        [SwaggerResponse(statusCode: 404, type: typeof(ErrorResponse), description: "Role Does Not Exist")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> DeleteRole([FromRoute] string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role is null)
            {
                //return NotFound(new ErrorResponse("Role Does Not Exist"));
                return NotFound();
            }

            await _roleManager.DeleteAsync(role);
            return Ok();
        }
    }
}
