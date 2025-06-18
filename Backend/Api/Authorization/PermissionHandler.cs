using Microsoft.AspNetCore.Authorization;

namespace Final_year_Project.Api.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var requiredPermission = requirement.Permission.ToLower();
            var userPermissions = context.User.FindAll("permission")
                                             .Select(c => c.Value.ToLower())
                                             .ToList();

            if (userPermissions.Contains(requiredPermission))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
