using Microsoft.AspNetCore.Authorization;

namespace Turnament.Authorization;

public static class AuthorizationSetup
{
    public static IServiceCollection AddAppAuthorization(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        
        services.AddScoped<IAuthorizationHandler, TeamAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, TournamentAuthorizationHandler>();
        
        services.AddAuthorization(options =>
        {
            options.AddPolicy("TeamCreator", policy => 
                policy.Requirements.Add(new TeamRequirement("Creator")));
            
            options.AddPolicy("TeamMember", policy => 
                policy.Requirements.Add(new TeamRequirement("Member")));
            
            options.AddPolicy("TournamentCreator", policy => 
                policy.Requirements.Add(new TournamentRequirement("Creator")));
            
            options.AddPolicy("TeamTournamentCreator", policy =>
            {
                policy.Requirements.Add(new TeamRequirement("Creator"));
                policy.Requirements.Add(new TournamentRequirement("Creator"));
            });
            
        });

        return services;
    }
}