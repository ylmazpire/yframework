using Microsoft.Extensions.Logging;

namespace YFramework.Monitoring.Alerting;

/// <summary>
/// Kayıtlı tüm <see cref="IAlertChannel"/>'lara uyarı iletir — ama yalnızca durum
/// değiştiğinde (bkz. <see cref="AlertDecision"/>). Bir kanal hata verirse diğerleri
/// yine denenir.
/// </summary>
public class AlertDispatcher
{
    private readonly IEnumerable<IAlertChannel> _kanallar;
    private readonly ILogger<AlertDispatcher> _logger;

    public AlertDispatcher(IEnumerable<IAlertChannel> kanallar, ILogger<AlertDispatcher> logger)
    {
        _kanallar = kanallar;
        _logger = logger;
    }

    /// <summary>
    /// <paramref name="onceki"/> → <paramref name="simdi"/> geçişi uyarı gerektiriyorsa gönderir.
    /// Gönderim yapıldıysa <c>true</c> döner.
    /// </summary>
    public async Task<bool> DispatchAsync(
        CheckOutcome? onceki,
        CheckOutcome simdi,
        Func<AlertTransition, AlertMessage> mesajOlustur,
        CancellationToken cancellationToken = default)
    {
        var gecis = AlertDecision.Evaluate(onceki, simdi);
        if (gecis == AlertTransition.None) return false;

        var mesaj = mesajOlustur(gecis);

        foreach (var kanal in _kanallar)
        {
            try
            {
                await kanal.SendAsync(mesaj, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Uyarı kanalı {Kanal} başarısız.", kanal.GetType().Name);
            }
        }

        return true;
    }
}
