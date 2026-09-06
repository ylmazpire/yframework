using System.Globalization;

namespace YFramework.Sequencing;

/// <summary>
/// Sıra numarasını okunur bir belge numarasına çevirir: <c>DocumentNumber.Format("2026", 42)</c> → <c>"2026-000042"</c>.
/// Fatura, iş emri, teklif numaraları için ortak biçimlendirme.
/// </summary>
public static class DocumentNumber
{
    public static string Format(long sira, int basamak = 6)
        => sira.ToString("D" + basamak, CultureInfo.InvariantCulture);

    public static string Format(string onEk, long sira, int basamak = 6, string ayirac = "-")
        => string.IsNullOrEmpty(onEk)
            ? Format(sira, basamak)
            : onEk + ayirac + Format(sira, basamak);
}
