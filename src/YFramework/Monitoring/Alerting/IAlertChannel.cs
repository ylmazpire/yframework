namespace YFramework.Monitoring.Alerting;

/// <summary>
/// Bir uyarıyı bir yere iletir (webhook, e-posta, SMS…). Uygulama hangi kanalları
/// kaydettiyse <see cref="AlertDispatcher"/> hepsine sırayla gönderir.
/// </summary>
public interface IAlertChannel
{
    Task SendAsync(AlertMessage mesaj, CancellationToken cancellationToken = default);
}
