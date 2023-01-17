using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CountryAPI.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RoleAuthorizationAttribute : BearerAuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string _roleToCheck;

        public RoleAuthorizationAttribute(string roleToCheck)
        {
            _roleToCheck = roleToCheck;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var jwtToken = context.HttpContext.Request.Headers["Authorization"].ToString()["Bearer ".Length..];
            var handler = new JwtSecurityTokenHandler();
            var decodedJwtToken = handler.ReadJwtToken(jwtToken);
            var isAuthorized = decodedJwtToken.Claims.Any(claim => claim.Value == _roleToCheck);
            if (!isAuthorized)
                context.Result = new UnauthorizedResult();
        }
    }
}
