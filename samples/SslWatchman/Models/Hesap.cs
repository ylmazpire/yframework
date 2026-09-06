using System.ComponentModel.DataAnnotations;

namespace SslWatchman.Models;

/// <summary>
/// Kiracının kendisi (bir hesap). <see cref="YFramework.MultiTenancy.ITenantScoped"/> değildir —
/// kiracıyı O tanımlar. "Kişisel kullanım" için tek kayıt olur; ileride servis olursa çoğalır.
/// </summary>
public class Hesap
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Ad { get; set; } = string.Empty;

    public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

    // --- hesap ayarları ---

    /// <summary>Durum değişince uyarı POST edilecek webhook (Slack/Discord uyumlu). Boşsa uyarı gönderilmez.</summary>
    [StringLength(500)]
    public string? WebhookUrl { get; set; }

    /// <summary>Sertifika bitişine bu kadar gün kala "uyarı" durumuna geçilir.</summary>
    [Range(1, 90)]
    public int UyariEsigiGun { get; set; } = 21;
}
