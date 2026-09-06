using System.ComponentModel.DataAnnotations;
using YFramework.MultiTenancy;
using YFramework.Validation;

namespace OtoServisApp.Models;

public class Musteri : ITenantScoped
{
    public int Id { get; set; }
    public int TenantId { get; set; }

    [Required(ErrorMessage = "Ad soyad / unvan zorunludur.")]
    [StringLength(200)]
    public string AdSoyad { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefon zorunludur.")]
    [TurkishPhoneNumber]
    [StringLength(20)]
    public string Telefon { get; set; } = string.Empty;

    /// <summary>
    /// Kurumsal müşteriler için vergi kimlik no (10 hane) ya da bireysel için TC kimlik no (11 hane).
    /// Fatura kesilirken gerekir; opsiyoneldir.
    /// </summary>
    [StringLength(11)]
    [TurkishTaxOrNationalId]
    public string? VergiKimlikNo { get; set; }

    public List<Arac> Araclar { get; set; } = new();
}
