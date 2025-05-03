using Microsoft.EntityFrameworkCore;
using Turnament.Data;
using Turnament.Models;

namespace Turnament.Authorization;

public class TournamentAuthorizationHandler(IHttpContextAccessor httpContextAccessor, AppDbContext context)
    : ResourceAuthorizationHandler<TournamentRequirement, Tournament>(httpContextAccessor, context)
{
    private readonly AppDbContext _context = context;

    protected override async Task<Tournament?> GetResourceAsync(string resourceId)
    {
        return await _context.Tournaments
            .FirstOrDefaultAsync(t => t.Id.ToString() == resourceId);
    }

    protected override async Task<bool> ValidateAccessAsync(Tournament resource, string userId, string requiredRole)
    {
        return requiredRole switch
        {
            "Creator" => resource.CreatorId.ToString() == userId,
            _ => false
        };
    }
}