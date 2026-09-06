using YFramework.Scheduling;

namespace YFramework.Tests.Scheduling;

public class ResourceSchedulerTests
{
    private record Kanal(int Id, string Ad);
    private record Plan(int KanalId, DateTime Baslangic, DateTime Bitis);

    private static DateTime T(int saat) => new(2026, 1, 5, saat, 0, 0);

    private static readonly Kanal[] Kanallar =
    {
        new(1, "Lift 1"), new(2, "Lift 2"), new(3, "Lift 3")
    };

    [Fact]
    public void AvailableResources_CakisanKanaliEler()
    {
        var planlar = new[]
        {
            new Plan(1, T(9), T(12)),   // Lift 1 dolu 09-12
            new Plan(2, T(13), T(15)),  // Lift 2 dolu 13-15
        };

        var bos = ResourceScheduler.AvailableResources(
            Kanallar, k => k.Id,
            planlar, p => p.KanalId, p => new TimeRange(p.Baslangic, p.Bitis),
            new TimeRange(T(10), T(11)));

        Assert.Equal(new[] { 2, 3 }, bos.Select(k => k.Id));
    }

    [Fact]
    public void AvailableResources_UclarDeginceCakismaSayilmaz()
    {
        var planlar = new[] { new Plan(1, T(9), T(12)) };

        var bos = ResourceScheduler.AvailableResources(
            Kanallar, k => k.Id,
            planlar, p => p.KanalId, p => new TimeRange(p.Baslangic, p.Bitis),
            new TimeRange(T(12), T(14)));

        Assert.Equal(new[] { 1, 2, 3 }, bos.Select(k => k.Id));
    }

    [Fact]
    public void FirstAvailable_SiradakiBosKanaliVerir()
    {
        var planlar = new[] { new Plan(1, T(9), T(18)) };

        var kanal = ResourceScheduler.FirstAvailable(
            Kanallar, k => k.Id,
            planlar, p => p.KanalId, p => new TimeRange(p.Baslangic, p.Bitis),
            new TimeRange(T(10), T(11)));

        Assert.NotNull(kanal);
        Assert.Equal(2, kanal!.Id);
    }

    [Fact]
    public void FirstAvailable_HepsiDoluysaNull()
    {
        var planlar = new[]
        {
            new Plan(1, T(8), T(19)), new Plan(2, T(8), T(19)), new Plan(3, T(8), T(19)),
        };

        var kanal = ResourceScheduler.FirstAvailable(
            Kanallar, k => k.Id,
            planlar, p => p.KanalId, p => new TimeRange(p.Baslangic, p.Bitis),
            new TimeRange(T(10), T(11)));

        Assert.Null(kanal);
    }

    [Fact]
    public void HasConflict_SadeceSeciliKaynaktakiPlanlaraBakar()
    {
        var lift1Planlari = new[] { new Plan(1, T(9), T(12)) };

        Assert.True(ResourceScheduler.HasConflict(
            lift1Planlari, p => new TimeRange(p.Baslangic, p.Bitis), new TimeRange(T(11), T(13))));
        Assert.False(ResourceScheduler.HasConflict(
            lift1Planlari, p => new TimeRange(p.Baslangic, p.Bitis), new TimeRange(T(12), T(13))));
    }
}
