using KuaforApp;
using KuaforApp.Data;
using KuaforApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YFramework.Auth;
using YFramework.Hosting;
using YFramework.MultiTenancy;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

// Oturum açan kullanıcının kiracı (işletme) bilgisini claim olarak ekler.
builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, TenantClaimsPrincipalFactory<ApplicationUser>>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// Statik dosya servisi + değişmez (invariant) kültür ayarı — bilinen tuzaklar için
// bkz. YFramework.Hosting.YFrameworkAppExtensions.
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
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Platform admini: hiçbir işletmeye bağlı değil (TenantId = null), tüm işletmeleri yönetir.
    await IdentitySeeder.SeedAsync(
        roleManager,
        userManager,
        new AdminSeedOptions("admin@kuafor.local", "Admin123!", "Platform Yöneticisi"));

    if (!await roleManager.RoleExistsAsync(AppRoles.IsletmeAdmini))
    {
        await roleManager.CreateAsync(new IdentityRole(AppRoles.IsletmeAdmini));
    }

    // Demo işletme + işletme admini (geliştirme/deneme amaçlı).
    var demoIsletme = await dbContext.Isletmeler.IgnoreQueryFilters().FirstOrDefaultAsync(i => i.Ad == "Zeynep Kuaför");
    if (demoIsletme is null)
    {
        demoIsletme = new Isletme { Ad = "Zeynep Kuaför" };
        dbContext.Isletmeler.Add(demoIsletme);
        await dbContext.SaveChangesAsync();
    }

    var isletmeAdminEmail = "isletme@kuafor.local";
    if (await userManager.FindByEmailAsync(isletmeAdminEmail) is null)
    {
        var isletmeAdmin = new ApplicationUser
        {
            UserName = isletmeAdminEmail,
            Email = isletmeAdminEmail,
            EmailConfirmed = true,
            AdSoyad = "Zeynep Yılmaz",
            TenantId = demoIsletme.Id
        };

        var result = await userManager.CreateAsync(isletmeAdmin, "Isletme123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(isletmeAdmin, AppRoles.IsletmeAdmini);
        }
    }

    if (!await dbContext.MuhasebeKategorileri.IgnoreQueryFilters()
            .AnyAsync(k => k.Ad == "Randevu Geliri" && k.TenantId == demoIsletme.Id))
    {
        dbContext.MuhasebeKategorileri.Add(new MuhasebeKategorisi
        {
            Ad = "Randevu Geliri",
            Tur = IslemTuru.Gelir,
            TenantId = demoIsletme.Id
        });
        await dbContext.SaveChangesAsync();
    }
}

app.Run();
