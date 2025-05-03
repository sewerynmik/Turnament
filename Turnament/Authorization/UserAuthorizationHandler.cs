using Microsoft.EntityFrameworkCore;
using Turnament.Data;
using Turnament.Models;

namespace Turnament.Authorization;

public class UserAuthorizationHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor) 
    : ResourceAuthorizationHandler<UserRequirement, User>(httpContextAccessor, context)
{
    private readonly AppDbContext _context = context;

    protected override Task<User?> GetResourceAsync(string resourceId)
    {
        return _context.Users
            .FirstOrDefaultAsync(u => u.Id.ToString() == resourceId);
    }

    protected override async Task<bool> ValidateAccessAsync(User resource, string userId, string requiredRole)
    {
        return requiredRole switch
        {
            "Creator" => resource.Id.ToString() == userId,
            _ => false
        };
    }
}