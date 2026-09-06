using YFramework.Validation;

namespace YFramework.Tests.Validation;

public class TurkishLicensePlateFormatterTests
{
    [Theory]
    [InlineData("34 abc 123", "34ABC123")]
    [InlineData("  06-AB-1234 ", "06AB1234")]
    [InlineData("01A4567", "01A4567")]
    public void Normalize_BoslukVeAyraclariAtarBuyukHarfeCevirir(string girdi, string beklenen)
        => Assert.Equal(beklenen, TurkishLicensePlateFormatter.Normalize(girdi));

    [Theory]
    [InlineData("34 A 1234")]
    [InlineData("34A12345")]
    [InlineData("06 AB 123")]
    [InlineData("06 AB 1234")]
    [InlineData("35 ABC 12")]
    [InlineData("35abc123")]
    [InlineData("81 Z 9999")]
    public void IsValid_GecerliPlakalar(string plaka)
        => Assert.True(TurkishLicensePlateFormatter.IsValid(plaka));

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("00 ABC 123")]   // il kodu 00
    [InlineData("82 ABC 123")]   // il kodu 82
    [InlineData("34 ABCD 123")]  // 4 harf
    [InlineData("34 A 123")]     // 1 harf + 3 rakam
    [InlineData("34 ABC 1")]     // 3 harf + 1 rakam
    [InlineData("34 ABC 1234")]  // 3 harf + 4 rakam
    public void IsValid_GecersizPlakalar(string? plaka)
        => Assert.False(TurkishLicensePlateFormatter.IsValid(plaka));

    [Fact]
    public void Bicimlendir_OkunurHaleGetirir()
        => Assert.Equal("34 ABC 123", TurkishLicensePlateFormatter.Bicimlendir("34abc123"));
}
