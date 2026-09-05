using System.Text.RegularExpressions;

namespace YFramework.Validation;

/// <summary>
/// Kullanıcının girdiği (boşluklu/farklı formatlı) Türkiye telefon numarasını
/// standart "0 5XX XXX XX XX" görünümüne çevirir. Doğrulama için değil, görüntüleme/saklama içindir.
/// </summary>
public static class TurkishPhoneNumberFormatter
{
    public static string Normalize(string input)
    {
        var digits = Regex.Replace(input, @"[^\d+]", "");

        if (digits.StartsWith("+90")) digits = "0" + digits[3..];
        else if (digits.StartsWith("0090")) digits = "0" + digits[4..];
        else if (digits.StartsWith("90") && digits.Length == 12) digits = "0" + digits[2..];

        if (digits.Length != 11 || !digits.StartsWith('0')) return input.Trim();

        return $"{digits[..1]} {digits[1..4]} {digits[4..7]} {digits[7..9]} {digits[9..11]}";
    }
}
