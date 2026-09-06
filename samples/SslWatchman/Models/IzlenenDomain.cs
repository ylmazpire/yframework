using System.ComponentModel.DataAnnotations;
using YFramework.Monitoring;
using YFramework.MultiTenancy;

namespace SslWatchman.Models;

/// <summary>
/// SSL sertifikası izlenen bir alan adı. <see cref="IMonitoredTarget"/> implemente eder;
/// arka plan servisi bunları periyodik kontrol edip <see cref="MonitorCheck"/> kaydı üretir.
/// Son durum alanları (SonSonuc, SertifikaBitisUtc…) hızlı pano + uyarı eşiği için denormalize.
/// </summary>
public class IzlenenDomain : ITenantScoped, IMonitoredTarget
{
    public int Id { get; set; }
    public int TenantId { get; set; }

    [Required(ErrorMessage = "Alan adı zorunludur.")]
    [StringLength(253)]
    public string Host { get; set; } = string.Empty;

    [Range(1, 65535, ErrorMessage = "Geçerli bir port girin.")]
    public int Port { get; set; } = 443;

    public bool Aktif { get; set; } = true;

    [StringLength(300)]
    public string? Not { get; set; }

    public DateTime EklenmeUtc { get; set; } = DateTime.UtcNow;

    // --- son kontrol özeti (denormalize) ---
    public CheckOutcome? SonSonuc { get; set; }
    public DateTime? SonKontrolUtc { get; set; }
    [StringLength(500)]
    public string? SonKontrolMesaji { get; set; }
    public DateTime? SertifikaBitisUtc { get; set; }
    [StringLength(200)]
    public string? Issuer { get; set; }

    // IMonitoredTarget
    public string TargetAdi => Port == 443 ? Host : $"{Host}:{Port}";

    public int? KalanGun => SertifikaBitisUtc is null
        ? null
        : (int)Math.Floor((SertifikaBitisUtc.Value - DateTime.UtcNow).TotalDays);
}
