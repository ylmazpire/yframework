using System.ComponentModel.DataAnnotations;

namespace YFramework.Validation;

/// <summary>
/// T.C. kimlik numarası doğrulaması (11 hane + resmi kontrol algoritması).
/// Boş değerleri geçerli sayar — zorunluluk kontrolü ayrıca [Required] ile yapılır.
/// </summary>
public class TurkishNationalIdAttribute : ValidationAttribute
{
    public TurkishNationalIdAttribute()
    {
        ErrorMessage = "Geçerli bir T.C. kimlik numarası girin (11 hane).";
    }

    public override bool IsValid(object? value)
    {
        if (value is not string input || string.IsNullOrWhiteSpace(input)) return true;
        return TurkishNationalIdValidator.IsValid(input.Trim());
    }
}
