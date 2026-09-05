using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using YFramework.Reporting;

namespace KuaforApp.Models;

public class MuhasebeCreateViewModel
{
    [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
    public int KategoriId { get; set; }

    [Range(0.01, 1000000, ErrorMessage = "Geçerli bir tutar girin.")]
    public decimal Tutar { get; set; }

    [StringLength(500)]
    public string Aciklama { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tarih zorunludur.")]
    public DateTime Tarih { get; set; } = DateTime.Today;

    public List<SelectListItem> Kategoriler { get; set; } = new();
}

public class MuhasebeRaporViewModel
{
    public int Yil { get; set; }
    public int Ay { get; set; }
    public FinancialSummary Ozet { get; set; } = new(0, 0);
    public List<MuhasebeKaydi> Kayitlar { get; set; } = new();
}
