namespace YFramework.Monitoring;

/// <summary>
/// Bir kontrolün sonucu. Sayısal sıra = ciddiyet sırası (Ok en düşük, Critical en yüksek) —
/// <see cref="Alerting.AlertDispatcher"/> bu sırayı "kötüleşti mi / düzeldi mi" kararında kullanır.
/// </summary>
public enum CheckOutcome
{
    /// <summary>Her şey yolunda.</summary>
    Ok = 0,

    /// <summary>Yakında sorun olacak (ör. sertifika 3 hafta içinde bitiyor).</summary>
    Warning = 1,

    /// <summary>Kontrol yapılamadı (bağlantı yok, TLS el sıkışması başarısız vb.).</summary>
    Error = 2,

    /// <summary>Şu an sorunlu (ör. sertifika süresi dolmuş, alan adı eşleşmiyor).</summary>
    Critical = 3
}
