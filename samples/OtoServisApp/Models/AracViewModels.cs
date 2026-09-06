using System.ComponentModel.DataAnnotations;
using YFramework.Validation;

namespace OtoServisApp.Models;

public abstract class AracFormViewModel
{
    [Display(Name = "Müşteri")]
    [Required(ErrorMessage = "Müşteri seçilmelidir.")]
    public int MusteriId { get; set; }

    [Display(Name = "Plaka")]
    [Required(ErrorMessage = "Plaka zorunludur.")]
    [StringLength(15)]
    [TurkishLicensePlate]
    public string Plaka { get; set; } = string.Empty;

    [Display(Name = "Marka")]
    [Required(ErrorMessage = "Marka zorunludur.")]
    [StringLength(50)]
    public string Marka { get; set; } = string.Empty;

    [Display(Name = "Model")]
    [Required(ErrorMessage = "Model zorunludur.")]
    [StringLength(50)]
    public string Model { get; set; } = string.Empty;

    [Display(Name = "Model yılı")]
    [Range(1950, 2100, ErrorMessage = "Geçerli bir model yılı girin.")]
    public int ModelYili { get; set; } = DateTime.Now.Year;

    [Display(Name = "Kilometre")]
    [Range(0, 5_000_000, ErrorMessage = "Geçerli bir kilometre girin.")]
    public int Kilometre { get; set; }
}

public class AracCreateViewModel : AracFormViewModel
{
}

public class AracEditViewModel : AracFormViewModel
{
    public int Id { get; set; }
}
