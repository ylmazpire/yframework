using YFramework.Validation;

namespace YFramework.Tests.Validation;

public class TurkishTaxNumberValidatorTests
{
    [Theory]
    [InlineData("1234567890")] // kontrol hanesi algoritmayla üretildi
    [InlineData("1111111114")]
    public void GecerliVknKabulEder(string value)
        => Assert.True(TurkishTaxNumberValidator.IsValid(value));

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("123456789")]    // 9 hane
    [InlineData("12345678901")]  // 11 hane
    [InlineData("1234567891")]   // kontrol hanesi bozuk
    [InlineData("1111111111")]   // kontrol hanesi bozuk
    [InlineData("12345678ab")]   // rakam değil
    public void GecersizVknReddeder(string? value)
        => Assert.False(TurkishTaxNumberValidator.IsValid(value));
}
