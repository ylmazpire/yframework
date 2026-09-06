using YFramework.Sequencing;

namespace YFramework.Tests.Sequencing;

public class DocumentNumberTests
{
    [Theory]
    [InlineData(42, 6, "000042")]
    [InlineData(1, 4, "0001")]
    [InlineData(1234567, 6, "1234567")] // basamak aşılırsa kırpılmaz
    public void Format_Padler(long sira, int basamak, string beklenen)
        => Assert.Equal(beklenen, DocumentNumber.Format(sira, basamak));

    [Theory]
    [InlineData("2026", 42, "2026-000042")]
    [InlineData("TEK", 7, "TEK-000007")]
    public void Format_OnEkli(string onEk, long sira, string beklenen)
        => Assert.Equal(beklenen, DocumentNumber.Format(onEk, sira));

    [Fact]
    public void Format_BosOnEk_AyracsizDoner()
        => Assert.Equal("000042", DocumentNumber.Format("", 42));
}
