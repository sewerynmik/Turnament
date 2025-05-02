namespace Turnament.Authorization;

public class TeamRequirement(string requiredRole) : IResourceRequirement
{
    public string ResourceParameterName => "id";
    public string RequiredRole { get; } = requiredRole;
}