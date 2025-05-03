namespace Turnament.Authorization;

public class UserRequirement(string requiredRole) : IResourceRequirement
{
    public string ResourceParameterName => "id";
    public string RequiredRole { get; } = requiredRole;
}