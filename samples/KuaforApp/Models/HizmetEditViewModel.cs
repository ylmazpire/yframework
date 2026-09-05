using System.ComponentModel.DataAnnotations;

namespace KuaforApp.Models;

public class HizmetEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Hizmet adı zorunludur.")]
    [StringLength(100)]
    public string Ad { get; set; } = string.Empty;

    [Range(5, 480, ErrorMessage = "Süre 5-480 dakika arasında olmalı.")]
    public int SureDakika { get; set; } = 30;

    [Range(0, 100000, ErrorMessage = "Geçerli bir fiyat girin.")]
    public decimal Fiyat { get; set; }
}
