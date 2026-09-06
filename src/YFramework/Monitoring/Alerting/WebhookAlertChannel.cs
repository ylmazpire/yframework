using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace YFramework.Monitoring.Alerting;

/// <summary>
/// Uyarıyı bir webhook URL'sine JSON POST eder. Varsayılan gövde Slack/Discord'un
/// <c>{ "content": ... }</c> / <c>{ "text": ... }</c> beklentisiyle uyumlu (ikisini de gönderir).
/// URL yapılandırılmamışsa sessizce hiçbir şey yapmaz.
/// </summary>
public class WebhookAlertChannel : IAlertChannel
{
    private readonly HttpClient _http;
    private readonly ILogger<WebhookAlertChannel> _logger;
    private readonly Func<string?> _urlAccessor;

    public WebhookAlertChannel(HttpClient http, ILogger<WebhookAlertChannel> logger, Func<string?> urlAccessor)
    {
        _http = http;
        _logger = logger;
        _urlAccessor = urlAccessor;
    }

    public async Task SendAsync(AlertMessage mesaj, CancellationToken cancellationToken = default)
    {
        var url = _urlAccessor();
        if (string.IsNullOrWhiteSpace(url)) return;

        var metin = $"[{mesaj.Ciddiyet}] {mesaj.Baslik}\n{mesaj.Govde}";
        var payload = new { content = metin, text = metin };

        try
        {
            using var resp = await _http.PostAsJsonAsync(url, payload, cancellationToken);
            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("Webhook uyarısı {Status} döndürdü.", (int)resp.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Webhook uyarısı gönderilemedi.");
        }
    }
}
