using System.ComponentModel.DataAnnotations;
using YFramework.MultiTenancy;

namespace OtoServisApp.Models;

/// <summary>
/// Parça kataloğu / stok kalemi. KuaforApp'te stok kavramı hiç olmadı — bu, framework'te genel bir
/// "stok hareketi" (StockLedger) kalıbına ihtiyaç olup olmadığını ortaya çıkaracak.
/// TODO(yframework): Genel stok hareketi kalıbı değerlendirilecek.
/// </summary>
public class Parca : ITenantScoped
{
    public int Id { get; set; }
    public int TenantId { get; set; }

    [Required(ErrorMessage = "Stok kodu zorunludur.")]
    [StringLength(50)]
    public string StokKodu { get; set; } = string.Empty;

    [Required(ErrorMessage = "Parça adı zorunludur.")]
    [StringLength(200)]
    public string Ad { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "Stok adedi negatif olamaz.")]
    public int StokAdedi { get; set; }

    [Range(0, 10_000_000, ErrorMessage = "Geçerli bir tutar girin.")]
    public decimal AlisFiyati { get; set; }

    [Range(0, 10_000_000, ErrorMessage = "Geçerli bir tutar girin.")]
    public decimal SatisFiyati { get; set; }
}
