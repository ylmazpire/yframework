namespace YFramework.Sequencing;

/// <summary>
/// Bir anahtar altında boşluksuz, artan numara üretir (fatura no, iş emri no, teklif no…).
/// KuaforApp'te hiç gerekmedi; oto serviste fatura numarası yıl + kiracı bazında sıralı olmalı.
///
/// Çok kiracılılık: bu arayüz kiracıyı BİLMEZ. Çağıran, anahtarı kendisi kurar —
/// ör. <c>$"fatura-{tenantId}-{DateTime.Now.Year}"</c>.
/// </summary>
public interface ISequenceGenerator
{
    /// <summary>Verilen anahtar için bir sonraki numarayı döndürür (ilk çağrıda 1).</summary>
    Task<long> NextAsync(string anahtar, CancellationToken cancellationToken = default);
}
