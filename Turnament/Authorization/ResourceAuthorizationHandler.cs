using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Turnament.Data;

namespace Turnament.Authorization;

public abstract class ResourceAuthorizationHandler<TRequirement, TResource> 
    : AuthorizationHandler<TRequirement> where TRequirement : IResourceRequirement
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private AppDbContext _context;

    protected ResourceAuthorizationHandler(IHttpContextAccessor httpContextAccessor, AppDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return;
        
        var routeData = httpContext.GetRouteData();
        var resourceId = routeData?.Values[requirement.ResourceParameterName]?.ToString();
        if (string.IsNullOrEmpty(resourceId)) return;

        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return;
        
        var resource = await GetResourceAsync(resourceId);
        if (resource == null) return;

        if (await ValidateAccessAsync(resource, userId, requirement.RequiredRole))
        {
            context.Succeed(requirement);
        }
    }
    
    protected abstract Task<TResource?> GetResourceAsync(string resourceId);
    protected abstract Task<bool> ValidateAccessAsync(TResource resource, string userId, string requiredRole);
}
