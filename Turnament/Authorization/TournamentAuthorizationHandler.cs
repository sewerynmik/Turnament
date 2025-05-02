using Microsoft.EntityFrameworkCore;
using Turnament.Data;
using Turnament.Models;

namespace Turnament.Authorization;

public class TournamentAuthorizationHandler : ResourceAuthorizationHandler<TournamentRequirement, Tournament>
{
    private readonly AppDbContext _context;
    
    public TournamentAuthorizationHandler(IHttpContextAccessor httpContextAccessor, AppDbContext context) : base(httpContextAccessor, context)
    {
        _context = context;
    }
    
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