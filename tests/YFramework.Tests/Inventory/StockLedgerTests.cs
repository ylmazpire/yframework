using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using YFramework.Inventory;

namespace YFramework.Tests.Inventory;

public class StockLedgerTests : IDisposable
{
    private sealed class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.AddYFrameworkStockLedger();
    }

    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<TestDbContext> _options;

    public StockLedgerTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        _options = new DbContextOptionsBuilder<TestDbContext>().UseSqlite(_connection).Options;
        using var db = new TestDbContext(_options);
        db.Database.EnsureCreated();
    }

    public void Dispose() => _connection.Dispose();

    [Fact]
    public async Task GirisCikis_EldekiMiktariHesaplar()
    {
        await using var db = new TestDbContext(_options);
        var ledger = new StockLedger(db);

        ledger.Giris(1, 100, "İlk alım");
        ledger.Cikis(1, 30, "İş emri", "IsEmri:5");
        ledger.Cikis(1, 10, "İş emri", "IsEmri:7");
        await db.SaveChangesAsync();

        Assert.Equal(60m, await ledger.EldekiMiktarAsync(1));
    }

    [Fact]
    public async Task HareketiOlmayanKalem_SifirDoner()
    {
        await using var db = new TestDbContext(_options);
        var ledger = new StockLedger(db);
        Assert.Equal(0m, await ledger.EldekiMiktarAsync(999));
    }

    [Fact]
    public async Task Cikis_HerZamanEksiYazar_GirisArti()
    {
        await using var db = new TestDbContext(_options);
        var ledger = new StockLedger(db);

        ledger.Giris(2, -5, "işaret önemsiz");   // yine +5
        ledger.Cikis(2, -2, "işaret önemsiz");   // yine -2
        await db.SaveChangesAsync();

        Assert.Equal(3m, await ledger.EldekiMiktarAsync(2));
        var hareketler = await ledger.HareketlerAsync(2);
        Assert.Equal(2, hareketler.Count);
        Assert.Contains(hareketler, h => h.Miktar == 5m);
        Assert.Contains(hareketler, h => h.Miktar == -2m);
    }

    [Fact]
    public async Task SifirMiktar_Reddedilir()
    {
        await using var db = new TestDbContext(_options);
        var ledger = new StockLedger(db);
        Assert.Throws<ArgumentException>(() => ledger.Giris(1, 0, "x"));
    }
}
