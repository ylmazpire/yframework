using System.Text.RegularExpressions;

namespace YFramework.Validation;

/// <summary>
/// Türkiye araç plakalarını normalize eder / doğrular / okunur biçime çevirir.
/// Saklamak için <see cref="Normalize"/> (boşluksuz, büyük harf) kullan; ekranda göstermek için
/// <see cref="Bicimlendir"/>.
/// </summary>
public static class TurkishLicensePlateFormatter
{
    // İl kodu 01-81, ardından 1-3 harf, ardından 2-5 rakam.
    private static readonly Regex YapiKalibi = new(
        @"^(0[1-9]|[1-7][0-9]|8[01])([A-Z]{1,3})([0-9]{2,5})$",
        RegexOptions.Compiled);

    /// <summary>Boşluk/ayraçları atar, büyük harfe çevirir (kültürden bağımsız). "34 abc 123" → "34ABC123".</summary>
    public static string Normalize(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        var sadeceAlfaNumerik = Regex.Replace(input, @"[^A-Za-z0-9]", "");
        return sadeceAlfaNumerik.ToUpperInvariant();
    }

    public static bool IsValid(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;

        var normalize = Normalize(input);
        var eslesme = YapiKalibi.Match(normalize);
        if (!eslesme.Success) return false;

        var harfSayisi = eslesme.Groups[2].Value.Length;
        var rakamSayisi = eslesme.Groups[3].Value.Length;

        // Resmî kombinasyonlar: 1 harf → 4-5 rakam, 2 harf → 3-4 rakam, 3 harf → 2-3 rakam.
        return harfSayisi switch
        {
            1 => rakamSayisi is 4 or 5,
            2 => rakamSayisi is 3 or 4,
            3 => rakamSayisi is 2 or 3,
            _ => false
        };
    }

    /// <summary>"34ABC123" → "34 ABC 123". Geçersizse girdiyi büyük harfe çevirip döndürür.</summary>
    public static string Bicimlendir(string input)
    {
        var normalize = Normalize(input);
        var eslesme = YapiKalibi.Match(normalize);
        if (!eslesme.Success) return normalize;

        return $"{eslesme.Groups[1].Value} {eslesme.Groups[2].Value} {eslesme.Groups[3].Value}";
    }
}
