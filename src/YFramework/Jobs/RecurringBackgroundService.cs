using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace YFramework.Jobs;

/// <summary>
/// Belirli aralıklarla bir işi tekrar çalıştıran arka plan servisi için taban sınıf.
/// KuaforApp / OtoServisApp'te hiç arka plan işi yoktu; SslWatchman'de sertifikaları
/// periyodik kontrol etmek gerekiyor — Lighthouse tarzı izleme araçları da bunu kullanır.
///
/// Bir iterasyonda hata olursa loglanır ve döngü DEVAM EDER (tek bir başarısız kontrol
/// tüm servisi düşürmesin).
/// </summary>
public abstract class RecurringBackgroundService : BackgroundService
{
    /// <summary>Türeyen sınıfların da kullanabilmesi için.</summary>
    protected ILogger Logger { get; }

    protected RecurringBackgroundService(ILogger logger)
    {
        Logger = logger;
    }

    /// <summary>İki çalıştırma arasındaki bekleme.</summary>
    protected abstract TimeSpan Interval { get; }

    /// <summary>Servis başladıktan sonra ilk çalıştırmaya kadar beklenecek süre (varsayılan: hemen).</summary>
    protected virtual TimeSpan InitialDelay => TimeSpan.Zero;

    /// <summary>İş adı — loglarda görünür.</summary>
    protected virtual string JobName => GetType().Name;

    /// <summary>Bir tur. İstisna atarsa yakalanır, loglanır, döngü devam eder.</summary>
    protected abstract Task ExecuteIterationAsync(CancellationToken cancellationToken);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (InitialDelay > TimeSpan.Zero)
        {
            try { await Task.Delay(InitialDelay, stoppingToken); }
            catch (OperationCanceledException) { return; }
        }

        using var timer = new PeriodicTimer(Interval);

        do
        {
            try
            {
                Logger.LogInformation("{Job}: tur başlıyor.", JobName);
                await ExecuteIterationAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "{Job}: tur sırasında hata — döngü devam ediyor.", JobName);
            }
        }
        while (await SafeWaitAsync(timer, stoppingToken));
    }

    private static async Task<bool> SafeWaitAsync(PeriodicTimer timer, CancellationToken token)
    {
        try { return await timer.WaitForNextTickAsync(token); }
        catch (OperationCanceledException) { return false; }
    }
}
