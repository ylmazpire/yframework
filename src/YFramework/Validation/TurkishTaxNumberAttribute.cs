using System.ComponentModel.DataAnnotations;

namespace YFramework.Validation;

/// <summary>
/// Vergi kimlik numarası doğrulaması (VKN, 10 hane + kontrol algoritması).
/// Boş değerleri geçerli sayar — zorunluluk kontrolü ayrıca [Required] ile yapılır.
/// </summary>
public class TurkishTaxNumberAttribute : ValidationAttribute
{
    public TurkishTaxNumberAttribute()
    {
        ErrorMessage = "Geçerli bir vergi kimlik numarası girin (10 hane).";
    }

    public override bool IsValid(object? value)
    {
        if (value is not string input || string.IsNullOrWhiteSpace(input)) return true;
        return TurkishTaxNumberValidator.IsValid(input.Trim());
    }
}
