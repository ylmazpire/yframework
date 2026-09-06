using System.ComponentModel.DataAnnotations;
using YFramework.Reporting;

namespace OtoServisApp.Models;

public class IsEmriCreateViewModel
{
    [Display(Name = "Araç")]
    [Required(ErrorMessage = "Araç seçilmelidir.")]
    public int AracId { get; set; }

    [Display(Name = "Müşteri şikâyeti / talebi")]
    [Required(ErrorMessage = "Müşteri şikâyeti / talebi girilmelidir.")]
    [StringLength(1000)]
    public string MusteriSikayeti { get; set; } = string.Empty;

    [Display(Name = "Geliş kilometresi")]
    [Range(0, 5_000_000, ErrorMessage = "Geçerli bir kilometre girin.")]
    public int? GelisKilometresi { get; set; }
}

public class IsEmriPlanlaViewModel
{
    public int Id { get; set; }

    [Display(Name = "Servis kanalı")]
    [Required(ErrorMessage = "Servis kanalı seçin.")]
    public int ServisKanaliId { get; set; }

    [Display(Name = "Atanan usta")]
    public int? AtananPersonelId { get; set; }

    [Display(Name = "Başlangıç")]
    [Required(ErrorMessage = "Başlangıç zamanı girin.")]
    public DateTime PlanlananBaslangic { get; set; }

    [Display(Name = "Bitiş")]
    [Required(ErrorMessage = "Bitiş zamanı girin.")]
    public DateTime PlanlananBitis { get; set; }
}

public class KalemEkleViewModel
{
    public int IsEmriId { get; set; }

    [Display(Name = "Tür")]
    public KalemTuru Tur { get; set; } = KalemTuru.Iscilik;

    [Display(Name = "Katalog parçası")]
    public int? ParcaId { get; set; }

    [Display(Name = "Açıklama")]
    [Required(ErrorMessage = "Açıklama zorunludur.")]
    [StringLength(300)]
    public string Aciklama { get; set; } = string.Empty;

    [Display(Name = "Adet")]
    [Range(0.01, 100_000, ErrorMessage = "Adet 0'dan büyük olmalı.")]
    public decimal Adet { get; set; } = 1;

    [Display(Name = "Birim fiyat")]
    [Range(0, 10_000_000, ErrorMessage = "Geçerli bir birim fiyat girin.")]
    public decimal BirimFiyat { get; set; }

    [Display(Name = "KDV oranı")]
    [Range(0, 1, ErrorMessage = "KDV oranı 0 ile 1 arasında olmalı (ör. 0.20).")]
    public decimal KdvOrani { get; set; } = 0.20m;
}

public class IsEmriDetayViewModel
{
    public IsEmri IsEmri { get; set; } = null!;
    public List<IsEmriKalemi> Kalemler { get; set; } = new();

    /// <summary>Ara toplam / KDV kırılımı / genel toplam — YFramework.Reporting.InvoiceCalculator ile.</summary>
    public FaturaOzeti Ozet => InvoiceCalculator.Hesapla(Kalemler);

    /// <summary>Bu iş emrinden kesilmiş fatura (varsa).</summary>
    public Fatura? Fatura { get; set; }

    public bool FaturaKesilebilir =>
        Fatura is null
        && Kalemler.Count > 0
        && IsEmri.Durum is IsEmriDurumu.Tamamlandi or IsEmriDurumu.TeslimEdildi;

    public KalemEkleViewModel YeniKalem { get; set; } = new();
    public IsEmriPlanlaViewModel Plan { get; set; } = new();
}
