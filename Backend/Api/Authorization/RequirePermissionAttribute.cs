using Microsoft.AspNetCore.Authorization;

namespace Final_year_Project.Api.Authorization
{
    public class RequirePermissionAttribute : AuthorizeAttribute
    {
        public RequirePermissionAttribute(string method, string endpoint)
        {
            Policy = $"{method}:{endpoint}";
        }
    }
}