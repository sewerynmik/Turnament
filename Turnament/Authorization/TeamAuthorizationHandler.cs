using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Turnament.Data;

namespace Turnament.Authorization;

public class TeamCreatorRequirement : IAuthorizationRequirement
{
}

public class TeamAuthorizationHandler(IHttpContextAccessor httpContextAccessor, AppDbContext context)
    : AuthorizationHandler<TeamCreatorRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly AppDbContext _context = context;

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TeamCreatorRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return;
        }
        
        var routeData = httpContext.GetRouteData();
        var teamId = routeData?.Values["id"]?.ToString();

        if (string.IsNullOrEmpty(teamId))
        {
            return;
        }
        
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return;
        }

        var team = await _context.Teams.FirstOrDefaultAsync(t => t.Id.ToString() == teamId);
        if (team?.CreatorId.ToString() == userId)
        {
            context.Succeed(requirement);
        }

    }
}