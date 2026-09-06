namespace YFramework.Reporting;

public record FinancialSummary(decimal ToplamGelir, decimal ToplamGider)
{
    public decimal Net => ToplamGelir - ToplamGider;
}

/// <summary>Bir ayın finansal özeti — aylık trend/grafik verisi için.</summary>
public record AylikFinansal(int Yil, int Ay, FinancialSummary Ozet);

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

    /// <summary>
    /// [baslangic, bitisHaric) tarih aralığındaki işlemlerin özeti. Rapor ekranlarındaki
    /// serbest tarih filtresi için (ör. "1 Oca – 31 Mar").
    /// </summary>
    public static FinancialSummary DonemOzeti(IEnumerable<IFinancialTransaction> islemler, DateTime baslangic, DateTime bitisHaric)
    {
        var donemIslemleri = islemler.Where(i => i.Tarih >= baslangic && i.Tarih < bitisHaric);
        return Ozet(donemIslemleri);
    }

    /// <summary>
    /// <paramref name="ilkAy"/> ayından başlayarak <paramref name="aySayisi"/> aylık özet üretir
    /// (grafik/trend için). Her iki örnek uygulamada elle yazılan "son 6 ay" mantığının ortak hali.
    /// </summary>
    public static IReadOnlyList<AylikFinansal> AylikTrend(IEnumerable<IFinancialTransaction> islemler, DateTime ilkAy, int aySayisi)
    {
        if (aySayisi < 1) throw new ArgumentOutOfRangeException(nameof(aySayisi));

        var liste = islemler as ICollection<IFinancialTransaction> ?? islemler.ToList();
        var basAy = new DateTime(ilkAy.Year, ilkAy.Month, 1);

        var sonuc = new List<AylikFinansal>(aySayisi);
        for (var i = 0; i < aySayisi; i++)
        {
            var ay = basAy.AddMonths(i);
            sonuc.Add(new AylikFinansal(ay.Year, ay.Month, AylikOzet(liste, ay.Year, ay.Month)));
        }
        return sonuc;
    }
}
