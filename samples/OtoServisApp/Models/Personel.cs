using System.ComponentModel.DataAnnotations;
using YFramework.MultiTenancy;
using YFramework.Validation;

namespace OtoServisApp.Models;

/// <summary>
/// Serviste çalışan usta / teknisyen. KuaforApp.Personel kalıbının aynısı — bu, framework'ün
/// "personel" kavramını gerçekten domain'den bağımsız taşıyıp taşımadığının testi.
/// </summary>
public class Personel : ITenantScoped
{
    public int Id { get; set; }
    public int TenantId { get; set; }

    [Required(ErrorMessage = "Ad soyad zorunludur.")]
    [StringLength(200)]
    public string AdSoyad { get; set; } = string.Empty;

    [TurkishPhoneNumber]
    [StringLength(20)]
    public string? Telefon { get; set; }

    /// <summary>Uzmanlık alanı (ör. "Motor", "Kaporta", "Elektrik").</summary>
    [StringLength(100)]
    public string? Uzmanlik { get; set; }

    public bool Aktif { get; set; } = true;
}
