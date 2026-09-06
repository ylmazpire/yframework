using YFramework.Reporting;

namespace YFramework.Tests.Reporting;

public class FinancialSummaryCalculatorTests
{
    private record Islem(DateTime Tarih, decimal Tutar, bool GelirMi) : IFinancialTransaction;

    private static readonly Islem[] Ornek =
    {
        new(new DateTime(2026, 1, 10), 1000m, true),
        new(new DateTime(2026, 1, 20), 300m, false),
        new(new DateTime(2026, 2, 5), 500m, true),
        new(new DateTime(2026, 3, 1), 200m, false),
    };

    [Fact]
    public void Ozet_GelirGiderNet()
    {
        var o = FinancialSummaryCalculator.Ozet(Ornek);
        Assert.Equal(1500m, o.ToplamGelir);
        Assert.Equal(500m, o.ToplamGider);
        Assert.Equal(1000m, o.Net);
    }

    [Fact]
    public void DonemOzeti_AralikDisiniElemez()
    {
        // Ocak + Şubat (1 Oca – 1 Mar): gelir 1500, gider 300
        var o = FinancialSummaryCalculator.DonemOzeti(Ornek, new DateTime(2026, 1, 1), new DateTime(2026, 3, 1));
        Assert.Equal(1500m, o.ToplamGelir);
        Assert.Equal(300m, o.ToplamGider);
    }

    [Fact]
    public void AylikTrend_HerAyIcinNokta()
    {
        var trend = FinancialSummaryCalculator.AylikTrend(Ornek, new DateTime(2026, 1, 15), 3);

        Assert.Equal(3, trend.Count);
        Assert.Equal((2026, 1), (trend[0].Yil, trend[0].Ay));
        Assert.Equal(1000m, trend[0].Ozet.ToplamGelir);
        Assert.Equal(300m, trend[0].Ozet.ToplamGider);
        Assert.Equal(500m, trend[1].Ozet.ToplamGelir);
        Assert.Equal(200m, trend[2].Ozet.ToplamGider);
    }

    [Fact]
    public void AylikTrend_GecersizAySayisi()
        => Assert.Throws<ArgumentOutOfRangeException>(() =>
            FinancialSummaryCalculator.AylikTrend(Ornek, DateTime.Today, 0));
}
