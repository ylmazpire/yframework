using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SslWatchman;
using SslWatchman.Data;
using SslWatchman.Models;
using SslWatchman.Services;
using YFramework.Auth;
using YFramework.Hosting;
using YFramework.MultiTenancy;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentTenantProvider, HttpContextTenantProvider>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, TenantClaimsPrincipalFactory<ApplicationUser>>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// --- SSL izleme ---
builder.Services.AddHttpClient();
builder.Services.AddSingleton<SertifikaKontrolServisi>();
builder.Services.AddScoped<DomainKontrolOrkestratoru>();
builder.Services.AddHostedService<KontrolCalistirici>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseYFrameworkDefaults();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await db.Database.MigrateAsync();

    await IdentitySeeder.SeedAsync(roleManager, userManager,
        new AdminSeedOptions("admin@sslwatchman.local", "Admin123!", "Platform Yöneticisi"));

    if (!await roleManager.RoleExistsAsync(AppRoles.Yonetici))
        await roleManager.CreateAsync(new IdentityRole(AppRoles.Yonetici));

    if (app.Environment.IsDevelopment())
    {
        var hesap = await db.Hesaplar.IgnoreQueryFilters().FirstOrDefaultAsync();
        if (hesap is null)
        {
            hesap = new Hesap { Ad = "Kişisel" };
            db.Hesaplar.Add(hesap);
            await db.SaveChangesAsync();
        }

        var yoneticiEmail = "ben@sslwatchman.local";
        if (await userManager.FindByEmailAsync(yoneticiEmail) is null)
        {
            var u = new ApplicationUser
            {
                UserName = yoneticiEmail,
                Email = yoneticiEmail,
                EmailConfirmed = true,
                AdSoyad = "Yılmaz",
                TenantId = hesap.Id
            };
            if ((await userManager.CreateAsync(u, "Yonetici123!")).Succeeded)
                await userManager.AddToRoleAsync(u, AppRoles.Yonetici);
        }
    }
}

app.Run();
