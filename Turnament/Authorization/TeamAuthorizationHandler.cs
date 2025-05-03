using Microsoft.EntityFrameworkCore;
using Turnament.Data;
using Turnament.Models;

namespace Turnament.Authorization;

public class TeamAuthorizationHandler(IHttpContextAccessor httpContextAccessor, AppDbContext context)
    : ResourceAuthorizationHandler<TeamRequirement, Team>(httpContextAccessor, context)
{
    private readonly AppDbContext _context = context;

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