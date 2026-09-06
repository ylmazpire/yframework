using System.ComponentModel.DataAnnotations;

namespace YFramework.Inventory;

/// <summary>
/// Tek bir stok hareketi. <see cref="Miktar"/> işaretlidir: pozitif = giriş, negatif = çıkış.
/// Eldeki miktar, bir kalemin tüm hareketlerinin toplamıdır — böylece her değişimin
/// nedeni ve zamanı iz olarak kalır (materyalize sayaç tutan uygulamalar bunu ayrıca
/// hızlı okuma için kopyalayabilir).
/// </summary>
public class StockMovement
{
    public int Id { get; set; }

    /// <summary>Uygulamanın stok kalemi (ör. Parca) anahtarı.</summary>
    public int StokKalemId { get; set; }

    /// <summary>+ giriş, − çıkış.</summary>
    public decimal Miktar { get; set; }

    [MaxLength(200)]
    public string Sebep { get; set; } = string.Empty;

    /// <summary>İlgili kaydın referansı, ör. "IsEmri:42".</summary>
    [MaxLength(100)]
    public string? Referans { get; set; }

    public DateTime Zaman { get; set; } = DateTime.UtcNow;
}
