using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using YFramework.MultiTenancy;

namespace YFramework.Auth;

/// <summary>
/// Kullanıcı oturum açtığında, kiracı kimliğini (varsa) claim olarak ekler.
/// ICurrentTenantProvider bu claim'i okuyarak sorgu filtrelerini uygular.
/// </summary>
public class TenantClaimsPrincipalFactory<TUser> : UserClaimsPrincipalFactory<TUser, IdentityRole>
    where TUser : ApplicationUser
{
    public TenantClaimsPrincipalFactory(
        UserManager<TUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
    {
    }

    public override async Task<ClaimsPrincipal> CreateAsync(TUser user)
    {
        var principal = await base.CreateAsync(user);

        if (user.TenantId.HasValue && principal.Identity is ClaimsIdentity identity)
        {
            identity.AddClaim(new Claim(TenantClaimTypes.TenantId, user.TenantId.Value.ToString()));
        }

        return principal;
    }
}
