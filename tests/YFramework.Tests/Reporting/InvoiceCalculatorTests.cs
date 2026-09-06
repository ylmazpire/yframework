using YFramework.Reporting;

namespace YFramework.Tests.Reporting;

public class InvoiceCalculatorTests
{
    private record Satir(decimal Adet, decimal BirimFiyat, decimal KdvOrani) : IFaturaSatiri;

    [Fact]
    public void TekOran_AraToplamKdvGenelToplam()
    {
        var ozet = InvoiceCalculator.Hesapla(new[]
        {
            new Satir(1, 500m, 0.20m),   // işçilik
            new Satir(2, 75m, 0.20m),    // 2 adet parça
        });

        Assert.Equal(650m, ozet.AraToplam);
        Assert.Equal(130m, ozet.ToplamKdv);
        Assert.Equal(780m, ozet.GenelToplam);
        Assert.Single(ozet.KdvKirilimleri);
        Assert.Equal(0.20m, ozet.KdvKirilimleri[0].Oran);
    }

    [Fact]
    public void FarkliOranlar_AyriAyriKirilir()
    {
        var ozet = InvoiceCalculator.Hesapla(new[]
        {
            new Satir(1, 1000m, 0.20m),
            new Satir(1, 200m, 0.10m),
            new Satir(1, 100m, 0.01m),
        });

        Assert.Equal(3, ozet.KdvKirilimleri.Count);
        Assert.Equal(new[] { 0.01m, 0.10m, 0.20m }, ozet.KdvKirilimleri.Select(k => k.Oran));
        Assert.Equal(1300m, ozet.AraToplam);
        Assert.Equal(200m + 20m + 1m, ozet.ToplamKdv);
        Assert.Equal(1521m, ozet.GenelToplam);
    }

    [Fact]
    public void KdvOranBazindaGruplanip_BirKezYuvarlanir()
    {
        // Her satırda ayrı yuvarlansaydı: 0.333*0.20 = 0.0666 -> 0.07 x3 = 0.21
        // Oran bazında: matrah 0.999 -> 1.00, KDV 1.00*0.20 = 0.20
        var ozet = InvoiceCalculator.Hesapla(new[]
        {
            new Satir(1, 0.333m, 0.20m),
            new Satir(1, 0.333m, 0.20m),
            new Satir(1, 0.333m, 0.20m),
        });

        Assert.Equal(1.00m, ozet.AraToplam);
        Assert.Equal(0.20m, ozet.ToplamKdv);
    }

    [Fact]
    public void BosListe_SifirDoner()
    {
        var ozet = InvoiceCalculator.Hesapla(Array.Empty<Satir>());
        Assert.Equal(0m, ozet.GenelToplam);
        Assert.Empty(ozet.KdvKirilimleri);
    }

    [Fact]
    public void YarimYukariYuvarlar()
    {
        // matrah 12.345 -> 12.35 (AwayFromZero); KDV 12.35 * 0.20 = 2.47
        var ozet = InvoiceCalculator.Hesapla(new[] { new Satir(1, 12.345m, 0.20m) });
        Assert.Equal(12.35m, ozet.AraToplam);
        Assert.Equal(2.47m, ozet.ToplamKdv);
    }
}
