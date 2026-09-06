namespace YFramework.Monitoring;

/// <summary>
/// Periyodik olarak kontrol edilen bir şey (bir alan adı, bir sunucu, bir HTTP ucu…).
/// Uygulamalar kendi entity'lerinde implemente eder; izleme altyapısı bu arayüz üzerinden
/// hedefleri listeler ve sonuçları <see cref="MonitorCheck"/> olarak kaydeder.
/// </summary>
public interface IMonitoredTarget
{
    /// <summary><see cref="MonitorCheck.TargetId"/> ile eşleşen kimlik.</summary>
    int Id { get; }

    /// <summary>Loglarda / panoda görünen ad.</summary>
    string TargetAdi { get; }

    /// <summary>Pasifse kontrol edilmez.</summary>
    bool Aktif { get; }
}
