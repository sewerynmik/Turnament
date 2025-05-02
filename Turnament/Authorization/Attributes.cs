using Microsoft.AspNetCore.Authorization;

namespace Turnament.Authorization;

public class TeamCreatorAuthorizationAttribute : AuthorizeAttribute
{
    public TeamCreatorAuthorizationAttribute() => Policy = "TeamCreator";
}

public class TournamentCreatorAuthorizationAttribute : AuthorizeAttribute
{
    public TournamentCreatorAuthorizationAttribute() => Policy = "TournamentCreator";
}

public class TeamCreatorOrTournamentCreatorAuthorizationAttribute : AuthorizeAttribute
{
    public TeamCreatorOrTournamentCreatorAuthorizationAttribute() => Policy = "TeamTournamentCreator";
}

public class TeamMemberAuthorizationAttribute : AuthorizeAttribute
{
    public TeamMemberAuthorizationAttribute() => Policy = "TeamMember";
}
