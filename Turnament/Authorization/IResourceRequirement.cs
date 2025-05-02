using Microsoft.AspNetCore.Authorization;

namespace Turnament.Authorization;

public interface IResourceRequirement : IAuthorizationRequirement
{
    string ResourceParameterName { get; }
    string RequiredRole { get; }
}