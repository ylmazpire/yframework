using System.ComponentModel.DataAnnotations;
using YFramework.Reporting;

namespace KuaforApp.Models;

public class MuhasebeKaydi : IFinancialTransaction
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
    public int KategoriId { get; set; }
    public MuhasebeKategorisi? Kategori { get; set; }

    [Range(0.01, 1000000, ErrorMessage = "Geçerli bir tutar girin.")]
    public decimal Tutar { get; set; }

    public string Aciklama { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tarih zorunludur.")]
    public DateTime Tarih { get; set; } = DateTime.Today;

    public int? RandevuId { get; set; }
    public Randevu? Randevu { get; set; }

    public bool GelirMi => Kategori?.Tur == IslemTuru.Gelir;
}
