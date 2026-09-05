using System.ComponentModel.DataAnnotations;
using YFramework.MultiTenancy;
using YFramework.Validation;

namespace KuaforApp.Models;

public class Musteri : ITenantScoped
{
    public int Id { get; set; }
    public int TenantId { get; set; }

    [Required(ErrorMessage = "Ad soyad zorunludur.")]
    public string AdSoyad { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefon zorunludur.")]
    [TurkishPhoneNumber]
    public string Telefon { get; set; } = string.Empty;
}
