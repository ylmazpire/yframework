using Microsoft.AspNetCore.Identity;

namespace YFramework.Auth;

/// <summary>
/// Uygulamaların kendi kullanıcı alanlarını eklemek için türetebileceği temel kullanıcı sınıfı.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string AdSoyad { get; set; } = string.Empty;
}
