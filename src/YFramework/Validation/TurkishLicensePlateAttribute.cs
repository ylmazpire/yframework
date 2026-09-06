using System.ComponentModel.DataAnnotations;

namespace YFramework.Validation;

/// <summary>
/// Türkiye araç plakası doğrulaması. Boşluklu/boşluksuz, küçük/büyük harf girişleri kabul eder
/// (iç normalize ile). Boş değerleri geçerli sayar — zorunluluk kontrolü [Required] ile yapılır.
/// </summary>
public class TurkishLicensePlateAttribute : ValidationAttribute
{
    public TurkishLicensePlateAttribute()
    {
        ErrorMessage = "Geçerli bir plaka girin (örn. 34 ABC 123).";
    }

    public override bool IsValid(object? value)
    {
        if (value is not string input || string.IsNullOrWhiteSpace(input)) return true;
        return TurkishLicensePlateFormatter.IsValid(input);
    }
}
