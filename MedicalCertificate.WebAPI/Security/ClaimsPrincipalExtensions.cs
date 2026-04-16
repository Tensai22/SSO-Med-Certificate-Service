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
}
