using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OtoServisApp;
using OtoServisApp.Data;
using OtoServisApp.Models;
using YFramework.Auth;
using YFramework.Hosting;
using YFramework.Inventory;
using YFramework.MultiTenancy;
using YFramework.Sequencing;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentTenantProvider, HttpContextTenantProvider>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Kiracı + yıl bazlı sıralı fatura numarası üreteci (YFramework.Sequencing).
builder.Services.AddScoped<ISequenceGenerator>(sp => new EfSequenceGenerator(sp.GetRequiredService<AppDbContext>()));

// Parça stok hareketleri defteri (YFramework.Inventory).
builder.Services.AddScoped(sp => new StockLedger(sp.GetRequiredService<AppDbContext>()));

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

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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

    await dbContext.Database.MigrateAsync();

    // Platform admini: hiçbir işletmeye bağlı değil (TenantId = null), tüm işletmeleri yönetir.
    await IdentitySeeder.SeedAsync(
        roleManager,
        userManager,
        new AdminSeedOptions("admin@otoservis.local", "Admin123!", "Platform Yöneticisi"));

    if (!await roleManager.RoleExistsAsync(AppRoles.ServisAdmini))
    {
        await roleManager.CreateAsync(new IdentityRole(AppRoles.ServisAdmini));
    }

    // Demo işletme + servis admini SADECE geliştirme ortamında oluşturulur — bilinen
    // e-posta/şifre ile bir hesap gerçek (production) dağıtımda asla otomatik açılmamalı.
    if (app.Environment.IsDevelopment())
    {
        var demoIsletme = await dbContext.Isletmeler.IgnoreQueryFilters()
            .FirstOrDefaultAsync(i => i.Ad == "Usta Mehmet Oto Servis");
        if (demoIsletme is null)
        {
            demoIsletme = new Isletme { Ad = "Usta Mehmet Oto Servis" };
            dbContext.Isletmeler.Add(demoIsletme);
            await dbContext.SaveChangesAsync();
        }

        var servisAdminEmail = "servis@otoservis.local";
        if (await userManager.FindByEmailAsync(servisAdminEmail) is null)
        {
            var servisAdmin = new ApplicationUser
            {
                UserName = servisAdminEmail,
                Email = servisAdminEmail,
                EmailConfirmed = true,
                AdSoyad = "Mehmet Usta",
                TenantId = demoIsletme.Id
            };

            var result = await userManager.CreateAsync(servisAdmin, "Servis123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(servisAdmin, AppRoles.ServisAdmini);
            }
        }

        if (!await dbContext.ServisKanallari.IgnoreQueryFilters().AnyAsync(k => k.TenantId == demoIsletme.Id))
        {
            dbContext.ServisKanallari.AddRange(
                new ServisKanali { Ad = "Lift 1", TenantId = demoIsletme.Id },
                new ServisKanali { Ad = "Lift 2", TenantId = demoIsletme.Id },
                new ServisKanali { Ad = "Yıkama / Kanal", TenantId = demoIsletme.Id });
            await dbContext.SaveChangesAsync();
        }
    }
}

app.Run();
