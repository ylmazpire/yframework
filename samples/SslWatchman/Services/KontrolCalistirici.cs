using Microsoft.EntityFrameworkCore;
using SslWatchman.Data;
using YFramework.Jobs;

namespace SslWatchman.Services;

/// <summary>
/// Belirli aralıklarla tüm aktif domainleri kontrol eden arka plan servisi.
/// Aralık appsettings'ten (SslWatchman:KontrolAralikiDakika) gelir.
///
/// Not: arka planda HttpContext yok → ICurrentTenantProvider.TenantId null → kiracı sorgu
/// filtresi tüm kiracıların domainlerini görür (istediğimiz de bu).
/// </summary>
public class KontrolCalistirici : RecurringBackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _config;

    public KontrolCalistirici(IServiceScopeFactory scopeFactory, IConfiguration config, ILogger<KontrolCalistirici> logger)
        : base(logger)
    {
        _scopeFactory = scopeFactory;
        _config = config;
    }

    protected override TimeSpan Interval =>
        TimeSpan.FromMinutes(_config.GetValue("SslWatchman:KontrolAralikiDakika", 360));

    protected override TimeSpan InitialDelay =>
        TimeSpan.FromSeconds(_config.GetValue("SslWatchman:BaslangicGecikmesiSaniye", 10));

    protected override string JobName => "SSL kontrolü";

    protected override async Task ExecuteIterationAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var orkestratör = scope.ServiceProvider.GetRequiredService<DomainKontrolOrkestratoru>();

        var domainler = await db.IzlenenDomainler.Where(d => d.Aktif).ToListAsync(ct);

        foreach (var domain in domainler)
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                await orkestratör.KontrolEtAsync(domain, ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                Logger.LogError(ex, "{Host} kontrol edilemedi.", domain.Host);
            }
        }

        // Geçmişi buda: yapılandırılan günden eski MonitorCheck kayıtlarını sil.
        var saklamaGunu = _config.GetValue("SslWatchman:GecmisiSaklamaGunu", 90);
        var esik = DateTime.UtcNow.AddDays(-saklamaGunu);
        await db.MonitorChecks.Where(c => c.ZamanUtc < esik).ExecuteDeleteAsync(ct);
    }
}
