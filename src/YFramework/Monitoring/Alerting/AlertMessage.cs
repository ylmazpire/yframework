namespace YFramework.Monitoring.Alerting;

/// <summary>Bir uyarı kanalına gönderilecek mesaj.</summary>
public record AlertMessage(CheckOutcome Ciddiyet, string Baslik, string Govde)
{
    /// <summary>Uyarının hangi hedefle ilgili olduğu (opsiyonel, kanal isterse kullanır).</summary>
    public string? Kaynak { get; init; }
}
