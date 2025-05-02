namespace Turnament.Authorization;

public class TournamentRequirement(string requiredRole) : IResourceRequirement
{
    public string ResourceParameterName => "id";
    public string RequiredRole { get; } = requiredRole;
}