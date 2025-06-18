using Microsoft.AspNetCore.Authorization;

namespace Final_year_Project.Api.Authorization
{
    public class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return Task.FromResult(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build());
        }

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        {
            return Task.FromResult<AuthorizationPolicy>(null);
        }

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName.Contains(":"))
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddRequirements(new PermissionRequirement(policyName))
                    .Build();

                return Task.FromResult(policy);
            }

            return Task.FromResult<AuthorizationPolicy>(null);
        }
    }
}
