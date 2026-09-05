using Microsoft.AspNetCore.Http;

namespace YFramework.MultiTenancy;

/// <summary>
/// Mevcut kullanıcının kiracı kimliğini, oturum açarken eklenen claim üzerinden okur.
/// (bkz. YFramework.Auth.TenantClaimsPrincipalFactory)
/// </summary>
public class HttpContextTenantProvider : ICurrentTenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextTenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? TenantId
    {
        get
        {
            var claimValue = _httpContextAccessor.HttpContext?.User.FindFirst(TenantClaimTypes.TenantId)?.Value;
            return int.TryParse(claimValue, out var tenantId) ? tenantId : null;
        }
    }
}
