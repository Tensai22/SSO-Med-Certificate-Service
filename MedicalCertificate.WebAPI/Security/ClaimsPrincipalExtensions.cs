using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MedicalCertificate.WebAPI.Security;

public static class ClaimsPrincipalExtensions
{
    public static int? GetCurrentUserId(this ClaimsPrincipal principal)
    {
        var rawUserId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? principal.FindFirstValue(JwtRegisteredClaimNames.Sub);

        return int.TryParse(rawUserId, out var userId) ? userId : null;
    }

    public static bool IsRegistrar(this ClaimsPrincipal principal)
    {
        var roleId = principal.FindFirstValue("roleId");
        if (roleId == "1")
        {
            return true;
        }

        var roleValues = principal.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .Where(v => !string.IsNullOrWhiteSpace(v));

        return roleValues.Any(role =>
            RoleNames.RegistrarAliases.Contains(role, StringComparer.OrdinalIgnoreCase)
            || role.Contains("registr", StringComparison.OrdinalIgnoreCase)
            || role.Contains("регист", StringComparison.OrdinalIgnoreCase));
    }
}
