using Microsoft.Extensions.Logging.Abstractions;
using YFramework.Monitoring;
using YFramework.Monitoring.Alerting;

namespace YFramework.Tests.Monitoring;

public class AlertDispatcherTests
{
    private sealed class SayanKanal : IAlertChannel
    {
        public int Sayi;
        public AlertMessage? Son;
        public Task SendAsync(AlertMessage mesaj, CancellationToken ct = default)
        {
            Sayi++;
            Son = mesaj;
            return Task.CompletedTask;
        }
    }

    private sealed class PatlayanKanal : IAlertChannel
    {
        public Task SendAsync(AlertMessage mesaj, CancellationToken ct = default)
            => throw new InvalidOperationException("kanal bozuk");
    }

    private static AlertDispatcher Kur(params IAlertChannel[] kanallar)
        => new(kanallar, NullLogger<AlertDispatcher>.Instance);

    [Fact]
    public async Task DurumDegismezse_Gondermez()
    {
        var k = new SayanKanal();
        var gonderildi = await Kur(k).DispatchAsync(CheckOutcome.Ok, CheckOutcome.Ok,
            _ => new AlertMessage(CheckOutcome.Ok, "x", "y"));

        Assert.False(gonderildi);
        Assert.Equal(0, k.Sayi);
    }

    [Fact]
    public async Task Kotulesince_TumKanallaraGonderir()
    {
        var a = new SayanKanal();
        var b = new SayanKanal();
        var gonderildi = await Kur(a, b).DispatchAsync(CheckOutcome.Ok, CheckOutcome.Critical,
            gecis => new AlertMessage(CheckOutcome.Critical, $"geçiş {gecis}", "gövde"));

        Assert.True(gonderildi);
        Assert.Equal(1, a.Sayi);
        Assert.Equal(1, b.Sayi);
        Assert.Equal("geçiş Escalation", a.Son!.Baslik);
    }

    [Fact]
    public async Task BirKanalPatlarsa_DigerleriYineDener()
    {
        var saglam = new SayanKanal();
        var gonderildi = await Kur(new PatlayanKanal(), saglam).DispatchAsync(
            CheckOutcome.Critical, CheckOutcome.Ok,
            _ => new AlertMessage(CheckOutcome.Ok, "toparlandı", "gövde"));

        Assert.True(gonderildi);
        Assert.Equal(1, saglam.Sayi);
    }
}
