using System.ComponentModel.DataAnnotations;
using YFramework.Validation;

namespace OtoServisApp.Models;

public class PersonelFormViewModel
{
    public int Id { get; set; }

    [Display(Name = "Ad soyad")]
    [Required(ErrorMessage = "Ad soyad zorunludur.")]
    [StringLength(200)]
    public string AdSoyad { get; set; } = string.Empty;

    [Display(Name = "Telefon")]
    [TurkishPhoneNumber]
    [StringLength(20)]
    public string? Telefon { get; set; }

    [Display(Name = "Uzmanlık")]
    [StringLength(100)]
    public string? Uzmanlik { get; set; }

    [Display(Name = "Aktif")]
    public bool Aktif { get; set; } = true;
}
