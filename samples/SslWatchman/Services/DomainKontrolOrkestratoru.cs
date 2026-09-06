using Microsoft.EntityFrameworkCore;
using SslWatchman.Data;
using SslWatchman.Models;
using YFramework.Monitoring;
using YFramework.Monitoring.Alerting;

namespace SslWatchman.Services;

/// <summary>
/// Bir domain'i kontrol eder → MonitorCheck kaydı ekler → domain'in son-durum alanlarını
/// günceller → durum değiştiyse (ve hesabın webhook'u varsa) uyarı yollar. Hem arka plan
/// servisi (<see cref="KontrolCalistirici"/>) hem de "şimdi kontrol et" butonu bunu çağırır.
/// </summary>
public class DomainKontrolOrkestratoru
{
    private readonly AppDbContext _db;
    private readonly SertifikaKontrolServisi _kontrol;
    private readonly IHttpClientFactory _httpFactory;
    private readonly ILoggerFactory _loggerFactory;

    public DomainKontrolOrkestratoru(
        AppDbContext db,
        SertifikaKontrolServisi kontrol,
        IHttpClientFactory httpFactory,
        ILoggerFactory loggerFactory)
    {
        _db = db;
        _kontrol = kontrol;
        _httpFactory = httpFactory;
        _loggerFactory = loggerFactory;
    }

    public async Task<SertifikaKontrolSonucu> KontrolEtAsync(IzlenenDomain domain, CancellationToken ct = default)
    {
        var hesap = await _db.Hesaplar.IgnoreQueryFilters().FirstOrDefaultAsync(h => h.Id == domain.TenantId, ct);
        var esik = hesap?.UyariEsigiGun ?? 21;

        var onceki = domain.SonSonuc;
        var sonuc = await _kontrol.KontrolEtAsync(domain.Host, domain.Port, esik, ct);

        _db.MonitorChecks.Add(new MonitorCheck
        {
            TargetId = domain.Id,
            Sonuc = sonuc.Sonuc,
            Mesaj = sonuc.Mesaj,
            AyrintiJson = sonuc.ToJson(),
            ZamanUtc = DateTime.UtcNow
        });

        domain.SonSonuc = sonuc.Sonuc;
        domain.SonKontrolUtc = DateTime.UtcNow;
        domain.SonKontrolMesaji = sonuc.Mesaj;
        domain.SertifikaBitisUtc = sonuc.BitisUtc;
        domain.Issuer = sonuc.Issuer;

        await _db.SaveChangesAsync(ct);

        if (!string.IsNullOrWhiteSpace(hesap?.WebhookUrl))
        {
            var kanal = new WebhookAlertChannel(
                _httpFactory.CreateClient("uyari"),
                _loggerFactory.CreateLogger<WebhookAlertChannel>(),
                () => hesap.WebhookUrl);

            var dispatcher = new AlertDispatcher(
                new IAlertChannel[] { kanal },
                _loggerFactory.CreateLogger<AlertDispatcher>());

            await dispatcher.DispatchAsync(onceki, sonuc.Sonuc, gecis =>
            {
                var baslik = gecis == AlertTransition.Recovery
                    ? $"✓ {domain.TargetAdi} toparlandı"
                    : $"{Simge(sonuc.Sonuc)} {domain.TargetAdi}";
                return new AlertMessage(sonuc.Sonuc, baslik, sonuc.Mesaj) { Kaynak = domain.TargetAdi };
            }, ct);
        }

        return sonuc;
    }

    private static string Simge(CheckOutcome o) => o switch
    {
        CheckOutcome.Critical => "🔴",
        CheckOutcome.Error => "🟠",
        CheckOutcome.Warning => "🟡",
        _ => "🟢"
    };
}
