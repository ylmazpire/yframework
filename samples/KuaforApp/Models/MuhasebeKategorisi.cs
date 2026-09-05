using System.ComponentModel.DataAnnotations;
using YFramework.MultiTenancy;

namespace KuaforApp.Models;

public enum IslemTuru
{
    Gelir,
    Gider
}

public enum GiderPeriyodu
{
    /// <summary>İçecek, malzeme gibi tek seferlik/günlük giderler.</summary>
    TekSeferlik,

    /// <summary>Elektrik, su, kira gibi her ay tekrar eden sabit giderler.</summary>
    AylikSabit
}

public class MuhasebeKategorisi : ITenantScoped
{
    public int Id { get; set; }
    public int TenantId { get; set; }

    [Required(ErrorMessage = "Kategori adı zorunludur.")]
    [StringLength(100)]
    public string Ad { get; set; } = string.Empty;

    [Required]
    public IslemTuru Tur { get; set; }

    /// <summary>Sadece Tur=Gider için anlamlıdır. Gelir kategorilerinde kullanılmaz.</summary>
    public GiderPeriyodu Periyot { get; set; } = GiderPeriyodu.TekSeferlik;
}
