using System.ComponentModel.DataAnnotations;
using YFramework.MultiTenancy;

namespace OtoServisApp.Models;

/// <summary>
/// Fiziksel bir çalışma alanı (lift / servis kanalı / bay). Bir işletmede birden fazla olur ve
/// her biri aynı anda tek bir araca ayrılır.
///
/// KuaforApp'te randevu tek bir personele bağlıydı; burada aynı anda birden fazla PARALEL kaynak
/// (N lift) var. Bu yüzden <see cref="YFramework.Scheduling.OverlapChecker"/> tek aralık kontrolü
/// yetmiyor — kaynak-farkındalıklı bir planlayıcıya ihtiyaç var.
/// TODO(yframework): ResourceScheduler / kaynak bazlı çakışma kontrolü YFramework.Scheduling'e eklenecek.
/// </summary>
public class ServisKanali : ITenantScoped
{
    public int Id { get; set; }
    public int TenantId { get; set; }

    [Required(ErrorMessage = "Kanal adı zorunludur.")]
    [StringLength(50)]
    public string Ad { get; set; } = string.Empty;

    public bool Aktif { get; set; } = true;
}
