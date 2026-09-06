using System.ComponentModel.DataAnnotations;

namespace OtoServisApp.Models;

public class GiderFormViewModel
{
    public int Id { get; set; }

    [Display(Name = "Açıklama")]
    [Required(ErrorMessage = "Açıklama zorunludur.")]
    [StringLength(300)]
    public string Aciklama { get; set; } = string.Empty;

    [Display(Name = "Tutar")]
    [Range(0.01, 100_000_000, ErrorMessage = "Geçerli bir tutar girin.")]
    public decimal Tutar { get; set; }

    [Display(Name = "Tarih")]
    [DataType(DataType.Date)]
    public DateTime Tarih { get; set; } = DateTime.Today;

    [Display(Name = "Kategori")]
    public GiderKategorisi Kategori { get; set; } = GiderKategorisi.Diger;
}
