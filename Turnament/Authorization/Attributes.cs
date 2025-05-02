using Microsoft.AspNetCore.Authorization;

namespace Turnament.Authorization;

public class TeamCreatorAuthorizationAttribute : AuthorizeAttribute
{
    public TeamCreatorAuthorizationAttribute() => Policy = "TeamCreator";
}

public class TeamMemberAuthorizationAttribute : AuthorizeAttribute
{
    public TeamMemberAuthorizationAttribute() => Policy = "TeamMember";
}
