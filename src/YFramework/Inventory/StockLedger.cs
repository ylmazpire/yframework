using Microsoft.EntityFrameworkCore;

namespace YFramework.Inventory;

/// <summary>
/// Stok hareketlerini (<see cref="StockMovement"/>) kaydeder ve eldeki miktarı hesaplar.
/// KuaforApp'te stok kavramı yoktu; oto serviste parça iş emrine eklenince stok düşmeli,
/// kalem silinince geri dönmeli — bu, tekrar eden bir kalıp olduğu için framework'e alındı.
///
/// Uygulamanın DbContext'i <see cref="StockMovement"/>'ı tanımalıdır
/// (<see cref="InventoryModelBuilderExtensions.AddYFrameworkStockLedger"/>).
/// <see cref="SaveChangesAsync"/>'i çağıran taraf yönetir (controller kendi transaction'ında
/// diğer değişikliklerle birlikte kaydeder) — bu yüzden metotlar yalnızca ekleme yapar.
/// </summary>
public class StockLedger
{
    private readonly DbContext _db;

    public StockLedger(DbContext db)
    {
        _db = db;
    }

    /// <summary>Stok girişi kaydeder (miktar &gt; 0 verilir, artı olarak yazılır).</summary>
    public void Giris(int stokKalemId, decimal miktar, string sebep, string? referans = null)
        => Ekle(stokKalemId, +Math.Abs(miktar), sebep, referans);

    /// <summary>Stok çıkışı kaydeder (miktar &gt; 0 verilir, eksi olarak yazılır).</summary>
    public void Cikis(int stokKalemId, decimal miktar, string sebep, string? referans = null)
        => Ekle(stokKalemId, -Math.Abs(miktar), sebep, referans);

    private void Ekle(int stokKalemId, decimal imzaliMiktar, string sebep, string? referans)
    {
        if (imzaliMiktar == 0) throw new ArgumentException("Miktar sıfır olamaz.", nameof(imzaliMiktar));
        _db.Set<StockMovement>().Add(new StockMovement
        {
            StokKalemId = stokKalemId,
            Miktar = imzaliMiktar,
            Sebep = sebep,
            Referans = referans,
            Zaman = DateTime.UtcNow
        });
    }

    /// <summary>Bir kalemin tüm hareketlerinin toplamı = eldeki miktar.</summary>
    public async Task<decimal> EldekiMiktarAsync(int stokKalemId, CancellationToken cancellationToken = default)
    {
        var hareketler = _db.Set<StockMovement>().Where(m => m.StokKalemId == stokKalemId);
        return await hareketler.AnyAsync(cancellationToken)
            ? await hareketler.SumAsync(m => m.Miktar, cancellationToken)
            : 0m;
    }

    public Task<List<StockMovement>> HareketlerAsync(int stokKalemId, CancellationToken cancellationToken = default)
        => _db.Set<StockMovement>()
            .Where(m => m.StokKalemId == stokKalemId)
            .OrderByDescending(m => m.Zaman).ThenByDescending(m => m.Id)
            .ToListAsync(cancellationToken);
}
