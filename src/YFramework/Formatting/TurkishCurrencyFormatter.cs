using System.Globalization;

namespace YFramework.Formatting;

/// <summary>
/// Para tutarlarını Türk Lirası formatında (₺1.234,56) görüntülemek için kullanılır.
/// Uygulamanın istek işleme kültürü (form/input parsing için InvariantCulture olmalı,
/// bkz. Program.cs) ile görüntüleme formatını birbirinden ayırmak için vardır —
/// aksi halde HTML5 "number" input'larının her zaman nokta ile gönderdiği değerler
/// sunucu kültürüne göre yanlış parse edilebilir (örn. "45.50" → 4550).
/// </summary>
public static class TurkishCurrencyFormatter
{
    private static readonly CultureInfo TrCulture = CultureInfo.GetCultureInfo("tr-TR");

    public static string Format(decimal amount) => amount.ToString("C", TrCulture);
}
