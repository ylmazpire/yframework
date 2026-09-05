using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using YFramework.Validation;

namespace KuaforApp.Models;

public class RandevuCreateViewModel
{
    /// <summary>Mevcut bir müşteri seçilirse doldurulur.</summary>
    public int? MusteriId { get; set; }

    /// <summary>Yeni bir müşteri girilirse doldurulur (MusteriId ile birlikte kullanılmaz).</summary>
    public string? YeniMusteriAdi { get; set; }

    [TurkishPhoneNumber]
    public string? YeniMusteriTelefon { get; set; }

    [Required(ErrorMessage = "Hizmet seçimi zorunludur.")]
    public int HizmetId { get; set; }

    [Required(ErrorMessage = "Başlangıç zamanı zorunludur.")]
    public DateTime BaslangicZamani { get; set; } = DateTime.Now;

    public List<SelectListItem> Musteriler { get; set; } = new();
    public List<SelectListItem> Hizmetler { get; set; } = new();
}
