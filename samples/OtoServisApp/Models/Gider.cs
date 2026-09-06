using System.ComponentModel.DataAnnotations;
using YFramework.MultiTenancy;
using YFramework.Reporting;

namespace OtoServisApp.Models;

public enum GiderKategorisi
{
    ParcaAlimi = 0,
    Kira = 1,
    Maas = 2,
    FaturaOdemesi = 3,   // elektrik, su, doğalgaz, internet…
    Diger = 4
}

/// <summary>
/// Servisin gideri. Gelir tarafı kesilen faturalardan gelir (<see cref="Fatura"/>);
/// ikisi de <see cref="IFinancialTransaction"/> implemente edip aynı rapora beslenir —
/// framework'ün <see cref="FinancialSummaryCalculator"/>'ının gerçekten domain-bağımsız
/// olduğunun testi.
/// </summary>
public class Gider : ITenantScoped, IFinancialTransaction
{
    public int Id { get; set; }
    public int TenantId { get; set; }

    [Required(ErrorMessage = "Açıklama zorunludur.")]
    [StringLength(300)]
    public string Aciklama { get; set; } = string.Empty;

    [Range(0.01, 100_000_000, ErrorMessage = "Geçerli bir tutar girin.")]
    public decimal Tutar { get; set; }

    public DateTime Tarih { get; set; } = DateTime.Today;

    public GiderKategorisi Kategori { get; set; } = GiderKategorisi.Diger;

    public bool GelirMi => false;
}
