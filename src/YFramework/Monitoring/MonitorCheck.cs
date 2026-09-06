using System.ComponentModel.DataAnnotations;

namespace YFramework.Monitoring;

/// <summary>
/// Tek bir kontrolün kaydı. Hedef başına geçmiş burada birikir — panoda "son 24 saat",
/// "ne zaman bozuldu" gibi sorular buradan yanıtlanır.
/// </summary>
public class MonitorCheck
{
    public int Id { get; set; }

    /// <summary>Kontrol edilen hedefin kimliği (<see cref="IMonitoredTarget.Id"/>).</summary>
    public int TargetId { get; set; }

    public CheckOutcome Sonuc { get; set; }

    /// <summary>İnsan tarafından okunur özet (ör. "Sertifika 12 gün sonra doluyor").</summary>
    [MaxLength(500)]
    public string Mesaj { get; set; } = string.Empty;

    public DateTime ZamanUtc { get; set; } = DateTime.UtcNow;

    /// <summary>Opsiyonel yapılandırılmış ayrıntı (JSON) — ör. issuer, NotAfter, SAN listesi.</summary>
    public string? AyrintiJson { get; set; }
}
