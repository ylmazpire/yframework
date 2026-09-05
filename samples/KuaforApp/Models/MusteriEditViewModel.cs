using System.ComponentModel.DataAnnotations;
using YFramework.Validation;

namespace KuaforApp.Models;

public class MusteriEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ad soyad zorunludur.")]
    [StringLength(200)]
    public string AdSoyad { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefon zorunludur.")]
    [TurkishPhoneNumber]
    public string Telefon { get; set; } = string.Empty;
}
