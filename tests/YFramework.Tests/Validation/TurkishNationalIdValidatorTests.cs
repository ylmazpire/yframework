using YFramework.Validation;

namespace YFramework.Tests.Validation;

public class TurkishNationalIdValidatorTests
{
    [Theory]
    [InlineData("10000000146")] // resmi algoritmaya uyan örnek
    [InlineData("19191919190")]
    public void GecerliNumaralari_KabulEder(string value)
        => Assert.True(TurkishNationalIdValidator.IsValid(value));

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("1234567890")]   // 10 hane
    [InlineData("123456789012")] // 12 hane
    [InlineData("01234567890")]  // ilk hane 0
    [InlineData("10000000147")]  // 11. hane bozuk
    [InlineData("10000000246")]  // 10. hane bozuk
    [InlineData("1000000014a")]  // rakam değil
    public void GecersizNumaralari_Reddeder(string? value)
        => Assert.False(TurkishNationalIdValidator.IsValid(value));
}
