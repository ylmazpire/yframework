using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace YFramework.Validation;

/// <summary>
/// Türkiye cep/sabit telefon numarası doğrulaması (0 5XX XXX XX XX gibi boşluklu girişleri de kabul eder).
/// Geçerli sayılması için: rakam dışı karakterler temizlendiğinde 11 haneli olmalı ve 0 ile başlamalı,
/// ya da +90/0090 ile başlayan 10 haneli olmalı.
/// </summary>
public class TurkishPhoneNumberAttribute : ValidationAttribute
{
    public TurkishPhoneNumberAttribute()
    {
        ErrorMessage = "Geçerli bir telefon numarası girin (örn. 0 533 598 32 12).";
    }

    public override bool IsValid(object? value)
    {
        if (value is not string input || string.IsNullOrWhiteSpace(input)) return true; // [Required] ayrı kontrol eder

        var digits = Regex.Replace(input, @"[^\d+]", "");

        if (digits.StartsWith("+90")) digits = "0" + digits[3..];
        else if (digits.StartsWith("0090")) digits = "0" + digits[4..];
        else if (digits.StartsWith("90") && digits.Length == 12) digits = "0" + digits[2..];

        return digits.Length == 11 && digits.StartsWith('0') && digits.Skip(1).All(char.IsDigit);
    }
}
