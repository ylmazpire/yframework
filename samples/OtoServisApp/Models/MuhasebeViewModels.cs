using YFramework.Reporting;

namespace OtoServisApp.Models;

/// <summary>Rapor tablosu + CSV için düzleştirilmiş tek satır (fatura veya gider).</summary>
public record HareketSatiri(DateTime Tarih, string Tur, string Aciklama, bool GelirMi, decimal Tutar);

public class MuhasebeRaporViewModel
{
    public DateTime Baslangic { get; set; }
    public DateTime Bitis { get; set; }

    public FinancialSummary Ozet { get; set; } = new(0, 0);
    public IReadOnlyList<AylikFinansal> AylikTrend { get; set; } = new List<AylikFinansal>();
    public List<HareketSatiri> Hareketler { get; set; } = new();

    public static readonly string[] AyKisaltmalari =
        { "", "Oca", "Şub", "Mar", "Nis", "May", "Haz", "Tem", "Ağu", "Eyl", "Eki", "Kas", "Ara" };
}
