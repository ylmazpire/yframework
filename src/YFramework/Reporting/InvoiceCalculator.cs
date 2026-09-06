namespace YFramework.Reporting;

/// <summary>
/// Bir fatura/adisyon satırının KDV hesabı için gereken minimum bilgi.
/// Uygulamalar kendi kalem sınıflarında (iş emri kalemi, sipariş satırı vb.) bu arayüzü implemente eder.
/// </summary>
public interface IFaturaSatiri
{
    decimal Adet { get; }
    decimal BirimFiyat { get; }

    /// <summary>KDV oranı ondalık olarak (ör. 0.20 = %20).</summary>
    decimal KdvOrani { get; }
}

/// <summary>Belirli bir KDV oranı için matrah (KDV hariç tutar) ve hesaplanan KDV.</summary>
public record KdvKirilimi(decimal Oran, decimal Matrah, decimal KdvTutari)
{
    public decimal DahilToplam => Matrah + KdvTutari;
}

public record FaturaOzeti(decimal AraToplam, decimal ToplamKdv, decimal GenelToplam, IReadOnlyList<KdvKirilimi> KdvKirilimleri);

/// <summary>
/// Satır bazında KDV taşıyan kalemlerden fatura özeti çıkarır. Kuaför projesindeki düz
/// gelir/gider (<see cref="FinancialSummaryCalculator"/>) yetmiyor: burada ara toplam,
/// KDV oranına göre kırılım ve genel toplam gerekiyor.
///
/// KDV, resmi faturalarda olduğu gibi ORAN BAZINDA gruplanıp hesaplanır (her satırda ayrı
/// yuvarlama yapmak yerine, aynı orandaki matrahlar toplanıp KDV bir kez hesaplanır).
/// Para tutarları 2 basamağa, bankacılıkta değil ticari hayatta standart olan
/// "yarımı yukarı" (AwayFromZero) kuralıyla yuvarlanır.
/// </summary>
public static class InvoiceCalculator
{
    public static FaturaOzeti Hesapla(IEnumerable<IFaturaSatiri> satirlar)
    {
        ArgumentNullException.ThrowIfNull(satirlar);

        var kirilimlar = satirlar
            .GroupBy(s => s.KdvOrani)
            .OrderBy(g => g.Key)
            .Select(g =>
            {
                var matrah = Yuvarla(g.Sum(s => s.Adet * s.BirimFiyat));
                var kdv = Yuvarla(matrah * g.Key);
                return new KdvKirilimi(g.Key, matrah, kdv);
            })
            .ToList();

        var araToplam = kirilimlar.Sum(k => k.Matrah);
        var toplamKdv = kirilimlar.Sum(k => k.KdvTutari);

        return new FaturaOzeti(araToplam, toplamKdv, araToplam + toplamKdv, kirilimlar);
    }

    private static decimal Yuvarla(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
