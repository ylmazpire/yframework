using System.ComponentModel.DataAnnotations;
using YFramework.Validation;

namespace KuaforApp.Models;

public class MusteriCreateViewModel
{
    [Required(ErrorMessage = "Ad soyad zorunludur.")]
    public string AdSoyad { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefon zorunludur.")]
    [TurkishPhoneNumber]
    public string Telefon { get; set; } = string.Empty;
}
