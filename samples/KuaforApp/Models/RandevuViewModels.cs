using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KuaforApp.Models;

public class RandevuCreateViewModel
{
    [Required(ErrorMessage = "Müşteri seçimi zorunludur.")]
    public int MusteriId { get; set; }

    [Required(ErrorMessage = "Hizmet seçimi zorunludur.")]
    public int HizmetId { get; set; }

    [Required(ErrorMessage = "Başlangıç zamanı zorunludur.")]
    public DateTime BaslangicZamani { get; set; } = DateTime.Now;

    public List<SelectListItem> Musteriler { get; set; } = new();
    public List<SelectListItem> Hizmetler { get; set; } = new();
}
