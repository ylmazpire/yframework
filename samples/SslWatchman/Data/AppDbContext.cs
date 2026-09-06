using Microsoft.EntityFrameworkCore;
using SslWatchman.Models;
using YFramework.Auth;
using YFramework.Monitoring;
using YFramework.MultiTenancy;

namespace SslWatchman.Data;

public class AppDbContext : YFrameworkIdentityDbContext<ApplicationUser>
{
    private readonly ICurrentTenantProvider _currentTenantProvider;

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentTenantProvider currentTenantProvider)
        : base(options)
    {
        _currentTenantProvider = currentTenantProvider;
    }

    public DbSet<Hesap> Hesaplar => Set<Hesap>();
    public DbSet<IzlenenDomain> IzlenenDomainler => Set<IzlenenDomain>();
    public DbSet<MonitorCheck> MonitorChecks => Set<MonitorCheck>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Kontrol geçmişi tablosu (YFramework.Monitoring).
        modelBuilder.AddYFrameworkMonitoring();

        // ITenantScoped implemente eden entity'lere otomatik kiracı filtresi + TenantId index'i.
        TenantQueryFilterApplier.Apply(modelBuilder, () => _currentTenantProvider.TenantId);

        // Aynı kiracı içinde host+port tekil.
        modelBuilder.Entity<IzlenenDomain>().HasIndex(d => new { d.TenantId, d.Host, d.Port }).IsUnique();
    }
}
