namespace YFramework.Reporting;

public record FinancialSummary(decimal ToplamGelir, decimal ToplamGider)
{
    public decimal Net => ToplamGelir - ToplamGider;
}

/// <summary>
/// Gelir/gider işlemlerinden dönem özetleri çıkaran genel amaçlı yardımcı sınıf.
/// Herhangi bir muhasebe/raporlama senaryosunda (yalnızca kuaför projesine özgü değil) kullanılabilir.
/// </summary>
public static class FinancialSummaryCalculator
{
    public static FinancialSummary Ozet(IEnumerable<IFinancialTransaction> islemler)
    {
        decimal gelir = 0, gider = 0;
        foreach (var islem in islemler)
        {
            if (islem.GelirMi) gelir += islem.Tutar;
            else gider += islem.Tutar;
        }
        return new FinancialSummary(gelir, gider);
    }

    public static FinancialSummary AylikOzet(IEnumerable<IFinancialTransaction> islemler, int yil, int ay)
    {
        var donemIslemleri = islemler.Where(i => i.Tarih.Year == yil && i.Tarih.Month == ay);
        return Ozet(donemIslemleri);
    }
}
