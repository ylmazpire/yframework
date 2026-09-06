using YFramework.MultiTenancy;
using YFramework.Reporting;

namespace OtoServisApp.Models;

/// <summary>
/// Bir iş emrinden kesilen fatura. Numarası kiracı + yıl bazında sıralıdır
/// (<see cref="YFramework.Sequencing.ISequenceGenerator"/>). Tutarlar, kesildiği andaki
/// anlık görüntüdür — iş emri kalemleri sonradan değişse bile fatura sabit kalır.
/// </summary>
public class Fatura : ITenantScoped, IFinancialTransaction
{
    public int Id { get; set; }
    public int TenantId { get; set; }

    public int IsEmriId { get; set; }
    public IsEmri? IsEmri { get; set; }

    public string FaturaNo { get; set; } = string.Empty;
    public DateTime Tarih { get; set; } = DateTime.Now;

    public decimal AraToplam { get; set; }
    public decimal ToplamKdv { get; set; }
    public decimal GenelToplam { get; set; }

    // IFinancialTransaction — fatura muhasebe raporunda gelir satırıdır.
    decimal IFinancialTransaction.Tutar => GenelToplam;
    bool IFinancialTransaction.GelirMi => true;
}
