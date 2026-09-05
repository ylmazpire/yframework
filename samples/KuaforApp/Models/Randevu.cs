using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YFramework.MultiTenancy;

namespace KuaforApp.Models;

public enum RandevuDurumu
{
    Planlandi,
    Tamamlandi,
    IptalEdildi
}

public class Randevu : ITenantScoped
{
    public int Id { get; set; }
    public int TenantId { get; set; }

    [Required]
    public int MusteriId { get; set; }
    public Musteri? Musteri { get; set; }

    [Required]
    public int HizmetId { get; set; }
    public Hizmet? Hizmet { get; set; }

    /// <summary>İşletmede personel tanımlıysa doldurulur. Tek kişilik işletmelerde null kalabilir.</summary>
    public int? PersonelId { get; set; }
    public Personel? Personel { get; set; }

    [Required(ErrorMessage = "Başlangıç zamanı zorunludur.")]
    public DateTime BaslangicZamani { get; set; }

    public RandevuDurumu Durum { get; set; } = RandevuDurumu.Planlandi;

    /// <summary>Kuaförün kendine özel notu (örn. saç boyası formülü). Opsiyonel.</summary>
    public string? Not { get; set; }

    [NotMapped]
    public DateTime BitisZamani => BaslangicZamani.AddMinutes(Hizmet?.SureDakika ?? 0);
}
