using YFramework.Scheduling;

namespace YFramework.Tests.Scheduling;

public class TimeRangeTests
{
    private static DateTime T(int saat) => new(2026, 1, 5, saat, 0, 0);

    [Theory]
    [InlineData(9, 12, 11, 13, true)]   // kısmi kesişim
    [InlineData(9, 12, 12, 13, false)]  // uçlar değiyor
    [InlineData(9, 12, 13, 14, false)]  // tamamen ayrı
    [InlineData(9, 17, 10, 11, true)]   // biri diğerini kapsıyor
    public void Overlaps(int s1, int e1, int s2, int e2, bool beklenen)
        => Assert.Equal(beklenen, new TimeRange(T(s1), T(e1)).Overlaps(new TimeRange(T(s2), T(e2))));

    [Fact]
    public void FromDuration_BitisiHesaplar()
    {
        var r = TimeRange.FromDuration(T(9), TimeSpan.FromHours(2));
        Assert.Equal(T(11), r.End);
        Assert.Equal(TimeSpan.FromHours(2), r.Duration);
    }

    [Theory]
    [InlineData(9, 12, true)]
    [InlineData(12, 12, false)]
    [InlineData(13, 12, false)]
    public void IsValid(int s, int e, bool beklenen)
        => Assert.Equal(beklenen, new TimeRange(T(s), T(e)).IsValid);
}
