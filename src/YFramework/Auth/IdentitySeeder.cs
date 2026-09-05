using Microsoft.AspNetCore.Identity;

namespace YFramework.Auth;

public record AdminSeedOptions(string Email, string Password, string AdSoyad);

/// <summary>
/// Uygulama açılışında rolleri ve (varsa) varsayılan admin kullanıcısını oluşturur.
/// Program.cs içinde uygulama başlarken bir kez çağrılır.
/// </summary>
public static class IdentitySeeder
{
    public static async Task SeedAsync<TUser>(
        RoleManager<IdentityRole> roleManager,
        UserManager<TUser> userManager,
        AdminSeedOptions? adminSeed = null)
        where TUser : IdentityUser, new()
    {
        foreach (var role in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        if (adminSeed is null) return;

        var existingAdmin = await userManager.FindByEmailAsync(adminSeed.Email);
        if (existingAdmin is not null) return;

        var admin = new TUser
        {
            UserName = adminSeed.Email,
            Email = adminSeed.Email,
            EmailConfirmed = true
        };

        if (admin is ApplicationUser applicationUser)
        {
            applicationUser.AdSoyad = adminSeed.AdSoyad;
        }

        var result = await userManager.CreateAsync(admin, adminSeed.Password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, Roles.Admin);
        }
    }
}
