using Microsoft.EntityFrameworkCore;
using Turnament.Data;
using Turnament.Models;

namespace Turnament.Authorization;

public class TeamAuthorizationHandler : ResourceAuthorizationHandler<TeamRequirement, Team>
{
    private readonly AppDbContext _context;
    
    public TeamAuthorizationHandler(IHttpContextAccessor httpContextAccessor, AppDbContext context) : base(httpContextAccessor, context)
    {
        _context = context;
    }
    
    protected override async Task<Team?> GetResourceAsync(string resourceId)
    {
        return await _context.Teams
            .FirstOrDefaultAsync(t => t.Id.ToString() == resourceId);
    }

    protected override async Task<bool> ValidateAccessAsync(Team resource, string userId, string requiredRole)
    {
        return requiredRole switch
        {
            "Creator" => resource.CreatorId.ToString() == userId,
            "Member" => await _context.TeamMembers
                .AnyAsync(tm => tm.TeamId == resource.Id && tm.UserId.ToString() == userId),
            _ => false
        };
    }
    
}