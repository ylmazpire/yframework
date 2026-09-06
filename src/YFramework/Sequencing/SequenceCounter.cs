using System.ComponentModel.DataAnnotations;

namespace YFramework.Sequencing;

/// <summary>
/// Adlandırılmış bir sayaç. "fatura-2026", "teklif-42-2026" gibi bir anahtar altında,
/// atomik biçimde artan bir tam sayı tutar. Çok kiracılı senaryolarda kiracı kimliği
/// anahtarın içine gömülür (bkz. <see cref="ISequenceGenerator"/>).
/// </summary>
public class SequenceCounter
{
    [Key]
    [MaxLength(200)]
    public string Anahtar { get; set; } = string.Empty;

    /// <summary>
    /// En son verilen numara. İlk <c>NextAsync</c> çağrısı 1 döndürür.
    /// Eşzamanlılık belirteci: iki istek aynı anda artırmaya çalışırsa biri
    /// <see cref="Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException"/> alır ve yeniden dener.
    /// </summary>
    public long SonDeger { get; set; }
}
