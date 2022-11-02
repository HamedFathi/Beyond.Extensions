// ReSharper disable CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Beyond.Extensions.ClaimsExtended;

public static class ClaimsExtensions
{
    public static IEnumerable<string> GetRoles(this ClaimsPrincipal claimsPrincipal)
    {
        if (claimsPrincipal == null) throw new ArgumentNullException(nameof(claimsPrincipal));
        var identity = claimsPrincipal.Identity;
        var result = (identity as ClaimsIdentity)?.GetRoles();
        return result ?? new List<string>();
    }

    public static IEnumerable<string> GetRoles(this ClaimsIdentity claimsIdentity)
    {
        if (claimsIdentity == null) throw new ArgumentNullException(nameof(claimsIdentity));

        var claims = claimsIdentity.Claims;
        var roles = claims.Where(c => c.Type == ClaimTypes.Role);
        return roles.Select(x => x.Value);
    }

    public static string? GetUserEmail(this ClaimsPrincipal claimsPrincipal)
    {
        if (claimsPrincipal == null) throw new ArgumentNullException(nameof(claimsPrincipal));
        var claim = claimsPrincipal.FindFirst(ClaimTypes.Email);
        return claim?.Value;
    }

    public static string? GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        if (claimsPrincipal == null) throw new ArgumentNullException(nameof(claimsPrincipal));
        var claim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
        return claim?.Value;
    }

    public static string? GetUserName(this ClaimsPrincipal claimsPrincipal)
    {
        if (claimsPrincipal == null) throw new ArgumentNullException(nameof(claimsPrincipal));
        var claim = claimsPrincipal.FindFirst(ClaimTypes.Name);
        return claim?.Value;
    }

    public static bool IsAuthenticated(this ClaimsPrincipal claimsPrincipal)
    {
        if (claimsPrincipal == null) throw new ArgumentNullException(nameof(claimsPrincipal));

        return claimsPrincipal is { Identity.IsAuthenticated: true };
    }
}