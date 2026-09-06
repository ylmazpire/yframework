using System.ComponentModel.DataAnnotations;
using YFramework.Validation;

namespace OtoServisApp.Models;

public class MusteriCreateViewModel
{
    [Display(Name = "Ad soyad / unvan")]
    [Required(ErrorMessage = "Ad soyad / unvan zorunludur.")]
    [StringLength(200)]
    public string AdSoyad { get; set; } = string.Empty;

    [Display(Name = "Telefon")]
    [Required(ErrorMessage = "Telefon zorunludur.")]
    [TurkishPhoneNumber]
    public string Telefon { get; set; } = string.Empty;

    [Display(Name = "Vergi / T.C. kimlik no")]
    [TurkishTaxOrNationalId]
    public string? VergiKimlikNo { get; set; }
}

public class MusteriEditViewModel
{
    public int Id { get; set; }

    [Display(Name = "Ad soyad / unvan")]
    [Required(ErrorMessage = "Ad soyad / unvan zorunludur.")]
    [StringLength(200)]
    public string AdSoyad { get; set; } = string.Empty;

    [Display(Name = "Telefon")]
    [Required(ErrorMessage = "Telefon zorunludur.")]
    [TurkishPhoneNumber]
    public string Telefon { get; set; } = string.Empty;

    [Display(Name = "Vergi / T.C. kimlik no")]
    [TurkishTaxOrNationalId]
    public string? VergiKimlikNo { get; set; }
}

public class MusteriDetayViewModel
{
    public Musteri Musteri { get; set; } = null!;
    public List<Arac> Araclar { get; set; } = new();
    public int ToplamIsEmriSayisi { get; set; }
}
