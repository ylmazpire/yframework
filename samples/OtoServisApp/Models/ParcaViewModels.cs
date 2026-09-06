using System.ComponentModel.DataAnnotations;

namespace OtoServisApp.Models;

public class ParcaFormViewModel
{
    public int Id { get; set; }

    [Display(Name = "Stok kodu")]
    [Required(ErrorMessage = "Stok kodu zorunludur.")]
    [StringLength(50)]
    public string StokKodu { get; set; } = string.Empty;

    [Display(Name = "Parça adı")]
    [Required(ErrorMessage = "Parça adı zorunludur.")]
    [StringLength(200)]
    public string Ad { get; set; } = string.Empty;

    [Display(Name = "Stok adedi")]
    [Range(0, int.MaxValue, ErrorMessage = "Stok adedi negatif olamaz.")]
    public int StokAdedi { get; set; }

    [Display(Name = "Alış fiyatı")]
    [Range(0, 10_000_000, ErrorMessage = "Geçerli bir tutar girin.")]
    public decimal AlisFiyati { get; set; }

    [Display(Name = "Satış fiyatı")]
    [Range(0, 10_000_000, ErrorMessage = "Geçerli bir tutar girin.")]
    public decimal SatisFiyati { get; set; }
}
