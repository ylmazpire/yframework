using System.ComponentModel.DataAnnotations;
using YFramework.MultiTenancy;

namespace OtoServisApp.Models;

public enum IsEmriDurumu
{
    Beklemede = 0,
    DevamEdiyor = 1,
    Tamamlandi = 2,
    TeslimEdildi = 3,
    Iptal = 4
}

/// <summary>
/// Bir araç için açılan iş emri (servis kaydı). KuaforApp.Randevu'nun karşılığı ama daha ağır:
/// altında kalemler (parça + işçilik) taşır ve bir servis kanalına + ustaya planlanır.
/// </summary>
public class IsEmri : ITenantScoped
{
    public int Id { get; set; }
    public int TenantId { get; set; }

    [Required(ErrorMessage = "Araç seçilmelidir.")]
    public int AracId { get; set; }
    public Arac? Arac { get; set; }

    public int? ServisKanaliId { get; set; }
    public ServisKanali? ServisKanali { get; set; }

    public int? AtananPersonelId { get; set; }
    public Personel? AtananPersonel { get; set; }

    public DateTime GelisTarihi { get; set; } = DateTime.Now;

    /// <summary>Servis kanalının bu iş için ayrıldığı zaman aralığı (planlama / çakışma kontrolü için).</summary>
    public DateTime? PlanlananBaslangic { get; set; }
    public DateTime? PlanlananBitis { get; set; }

    public IsEmriDurumu Durum { get; set; } = IsEmriDurumu.Beklemede;

    [Required(ErrorMessage = "Müşteri şikâyeti / talebi girilmelidir.")]
    [StringLength(1000)]
    public string MusteriSikayeti { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? YapilanIsler { get; set; }

    /// <summary>Araç servise geldiğindeki kilometre.</summary>
    [Range(0, 5_000_000)]
    public int? GelisKilometresi { get; set; }

    public List<IsEmriKalemi> Kalemler { get; set; } = new();
}
