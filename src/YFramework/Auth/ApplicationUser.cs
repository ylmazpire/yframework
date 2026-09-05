using Microsoft.AspNetCore.Identity;

namespace YFramework.Auth;

/// <summary>
/// Uygulamaların kendi kullanıcı alanlarını eklemek için türetebileceği temel kullanıcı sınıfı.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string AdSoyad { get; set; } = string.Empty;

    /// <summary>
    /// Kullanıcının bağlı olduğu kiracı (işletme). null ise platform admini demektir —
    /// hiçbir işletmeye bağlı değildir ve tüm kiracıların verilerini görebilir.
    /// </summary>
    public int? TenantId { get; set; }
}
