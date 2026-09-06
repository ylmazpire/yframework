using System.ComponentModel.DataAnnotations;
using YFramework.MultiTenancy;
using YFramework.Reporting;

namespace OtoServisApp.Models;

public enum KalemTuru
{
    Parca = 0,
    Iscilik = 1
}

/// <summary>
/// İş emri satırı: bir parça ya da bir işçilik kalemi. Satır bazında KDV oranı taşır.
///
/// KuaforApp'in finansal raporlaması düz gelir/gider'di (KDV yok, kalem yok). Burada fatura
/// = kalemler + satır KDV'si + genel toplam. Bu, YFramework.Reporting'in KDV/kalem farkındalığı
/// olan bir hesaplayıcıya ihtiyacı olduğunu gösteriyor.
/// TODO(yframework): InvoiceCalculator (ara toplam / KDV kırılımı / genel toplam) YFramework.Reporting'e eklenecek.
///
/// Not: Bu bir "child/owned" entity. Sahibi (<see cref="IsEmri"/>) kiracı filtresine tabi olduğu için
/// EF, ilişkinin iki ucunda da eşleşen filtre ister. En temiz çözüm: child'a da TenantId koyup
/// <see cref="ITenantScoped"/> yapmak — framework filtreyi + index'i otomatik ekliyor.
/// TODO(yframework): child entity'lerde TenantId denormalizasyonu framework'te kalıp haline getirilecek mi?
/// </summary>
public class IsEmriKalemi : ITenantScoped, IFaturaSatiri
{
    public int Id { get; set; }
    public int TenantId { get; set; }

    public int IsEmriId { get; set; }
    public IsEmri? IsEmri { get; set; }

    public KalemTuru Tur { get; set; }

    /// <summary>Katalogdan seçilen parça (varsa). İşçilik kalemlerinde null.</summary>
    public int? ParcaId { get; set; }
    public Parca? Parca { get; set; }

    [Required(ErrorMessage = "Açıklama zorunludur.")]
    [StringLength(300)]
    public string Aciklama { get; set; } = string.Empty;

    [Range(0.01, 100_000, ErrorMessage = "Adet 0'dan büyük olmalı.")]
    public decimal Adet { get; set; } = 1;

    [Range(0, 10_000_000, ErrorMessage = "Geçerli bir birim fiyat girin.")]
    public decimal BirimFiyat { get; set; }

    /// <summary>KDV oranı (ör. 0.20 = %20). Türkiye'de parça/işçilik genelde %20.</summary>
    [Range(0, 1, ErrorMessage = "KDV oranı 0 ile 1 arasında olmalı (ör. 0.20).")]
    public decimal KdvOrani { get; set; } = 0.20m;

    public decimal AraToplam => Adet * BirimFiyat;
    public decimal KdvTutari => AraToplam * KdvOrani;
    public decimal Toplam => AraToplam + KdvTutari;
}
