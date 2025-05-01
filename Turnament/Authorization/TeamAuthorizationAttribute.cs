using Microsoft.AspNetCore.Authorization;

namespace Turnament.Authorization;

public class TeamAuthorizationAttribute : AuthorizeAttribute
{
    public TeamAuthorizationAttribute()
    {
        Policy = "Team";
    }
    
}