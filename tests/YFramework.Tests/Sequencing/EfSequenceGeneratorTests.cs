using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using YFramework.Sequencing;

namespace YFramework.Tests.Sequencing;

public class EfSequenceGeneratorTests : IDisposable
{
    private sealed class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.AddYFrameworkSequences();
    }

    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<TestDbContext> _options;

    public EfSequenceGeneratorTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        _options = new DbContextOptionsBuilder<TestDbContext>().UseSqlite(_connection).Options;
        using var db = new TestDbContext(_options);
        db.Database.EnsureCreated();
    }

    public void Dispose() => _connection.Dispose();

    private TestDbContext NewContext() => new(_options);

    [Fact]
    public async Task NextAsync_1DenBaslar_VeArtar()
    {
        await using var db = NewContext();
        var gen = new EfSequenceGenerator(db);

        Assert.Equal(1, await gen.NextAsync("fatura-2026"));
        Assert.Equal(2, await gen.NextAsync("fatura-2026"));
        Assert.Equal(3, await gen.NextAsync("fatura-2026"));
    }

    [Fact]
    public async Task FarkliAnahtarlar_BagimsizSayilir()
    {
        await using var db = NewContext();
        var gen = new EfSequenceGenerator(db);

        Assert.Equal(1, await gen.NextAsync("fatura-1-2026"));
        Assert.Equal(1, await gen.NextAsync("fatura-2-2026"));
        Assert.Equal(2, await gen.NextAsync("fatura-1-2026"));
    }

    [Fact]
    public async Task YeniContext_KalinanYerdenDevamEder()
    {
        await using (var db1 = NewContext())
        {
            var gen1 = new EfSequenceGenerator(db1);
            await gen1.NextAsync("teklif-2026");
            await gen1.NextAsync("teklif-2026");
        }

        await using var db2 = NewContext();
        var gen2 = new EfSequenceGenerator(db2);
        Assert.Equal(3, await gen2.NextAsync("teklif-2026"));
    }
}
