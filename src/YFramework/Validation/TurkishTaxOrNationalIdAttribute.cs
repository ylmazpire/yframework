using System.ComponentModel.DataAnnotations;

namespace YFramework.Validation;

/// <summary>
/// Hem bireysel (T.C. kimlik no, 11 hane) hem kurumsal (VKN, 10 hane) müşterileri tek alanda
/// tutan senaryolar için: 10 haneyse VKN, 11 haneyse TCKN algoritmasıyla doğrular.
/// Boş değerleri geçerli sayar — zorunluluk kontrolü ayrıca [Required] ile yapılır.
/// </summary>
public class TurkishTaxOrNationalIdAttribute : ValidationAttribute
{
    public TurkishTaxOrNationalIdAttribute()
    {
        ErrorMessage = "Geçerli bir T.C. kimlik no (11 hane) veya vergi kimlik no (10 hane) girin.";
    }

    public override bool IsValid(object? value)
    {
        if (value is not string input || string.IsNullOrWhiteSpace(input)) return true;

        var s = input.Trim();
        return s.Length switch
        {
            10 => TurkishTaxNumberValidator.IsValid(s),
            11 => TurkishNationalIdValidator.IsValid(s),
            _ => false
        };
    }
}
