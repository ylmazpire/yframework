using KuaforApp.Models;
using Microsoft.EntityFrameworkCore;
using YFramework.Auth;
using YFramework.MultiTenancy;

namespace KuaforApp.Data;

public class AppDbContext : YFrameworkIdentityDbContext<ApplicationUser>
{
    private readonly ICurrentTenantProvider _currentTenantProvider;

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentTenantProvider currentTenantProvider)
        : base(options)
    {
        _currentTenantProvider = currentTenantProvider;
    }

    public DbSet<Isletme> Isletmeler => Set<Isletme>();
    public DbSet<Musteri> Musteriler => Set<Musteri>();
    public DbSet<Hizmet> Hizmetler => Set<Hizmet>();
    public DbSet<Personel> Personeller => Set<Personel>();
    public DbSet<Randevu> Randevular => Set<Randevu>();
    public DbSet<MuhasebeKategorisi> MuhasebeKategorileri => Set<MuhasebeKategorisi>();
    public DbSet<MuhasebeKaydi> MuhasebeKayitlari => Set<MuhasebeKaydi>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        TenantQueryFilterApplier.Apply(modelBuilder, () => _currentTenantProvider.TenantId);
    }
}
