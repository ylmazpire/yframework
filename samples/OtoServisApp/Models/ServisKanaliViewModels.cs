using System.ComponentModel.DataAnnotations;

namespace OtoServisApp.Models;

public class ServisKanaliFormViewModel
{
    public int Id { get; set; }

    [Display(Name = "Kanal adı")]
    [Required(ErrorMessage = "Kanal adı zorunludur.")]
    [StringLength(50)]
    public string Ad { get; set; } = string.Empty;

    [Display(Name = "Aktif")]
    public bool Aktif { get; set; } = true;
}
