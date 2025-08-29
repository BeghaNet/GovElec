namespace GovElec.Api.Auth;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


public class ScopeAuthorizationHandler : AuthorizationHandler<ScopeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ScopeRequirement requirement)
    {
        var scopes = context.User.FindFirstValue("scope") ?? string.Empty;
        // scopes séparés par espace
        if (scopes.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                  .Contains(requirement.RequiredScope))
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}

