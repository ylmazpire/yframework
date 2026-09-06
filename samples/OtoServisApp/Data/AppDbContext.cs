using Microsoft.EntityFrameworkCore;
using OtoServisApp.Models;
using YFramework.Auth;
using YFramework.Inventory;
using YFramework.MultiTenancy;
using YFramework.Sequencing;

namespace OtoServisApp.Data;

public class AppDbContext : YFrameworkIdentityDbContext<ApplicationUser>
{
    private readonly ICurrentTenantProvider _currentTenantProvider;

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentTenantProvider currentTenantProvider)
        : base(options)
    {
        _currentTenantProvider = currentTenantProvider;
    }

    public DbSet<Isletme> Isletmeler => Set<Isletme>();
    public DbSet<Musteri> Musteriler => Set<Musteri>();
    public DbSet<Arac> Araclar => Set<Arac>();
    public DbSet<Personel> Personeller => Set<Personel>();
    public DbSet<ServisKanali> ServisKanallari => Set<ServisKanali>();
    public DbSet<Parca> Parcalar => Set<Parca>();
    public DbSet<IsEmri> IsEmirleri => Set<IsEmri>();
    public DbSet<IsEmriKalemi> IsEmriKalemleri => Set<IsEmriKalemi>();
    public DbSet<Fatura> Faturalar => Set<Fatura>();
    public DbSet<Gider> Giderler => Set<Gider>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Kiracı + yıl bazlı sıralı fatura numarası için sayaç tablosu (YFramework.Sequencing).
        modelBuilder.AddYFrameworkSequences();

        // Parça stok hareketleri (YFramework.Inventory) — parça iş emrine eklenince çıkış, silinince giriş.
        modelBuilder.AddYFrameworkStockLedger();

        // ITenantScoped implemente eden HER entity'ye otomatik kiracı filtresi + TenantId index'i.
        TenantQueryFilterApplier.Apply(modelBuilder, () => _currentTenantProvider.TenantId);

        // İş emirleri neredeyse her sorguda tarih aralığı + duruma göre filtreleniyor
        // (bekleyen işler, planlama, gün listesi) — TenantId ile bileşik index.
        modelBuilder.Entity<IsEmri>().HasIndex(e => new { e.TenantId, e.PlanlananBaslangic });
        modelBuilder.Entity<IsEmri>().HasIndex(e => new { e.TenantId, e.GelisTarihi });

        // Plaka ile araç arama sık yapılır.
        modelBuilder.Entity<Arac>().HasIndex(a => new { a.TenantId, a.Plaka });

        // Stok kodu kiracı içinde tekil olmalı.
        modelBuilder.Entity<Parca>().HasIndex(p => new { p.TenantId, p.StokKodu }).IsUnique();

        // İş emri silinince kalemleri de silinsin; parça kataloğu silinince kalem ParcaId'si null olsun.
        modelBuilder.Entity<IsEmriKalemi>()
            .HasOne(k => k.IsEmri)
            .WithMany(e => e.Kalemler)
            .HasForeignKey(k => k.IsEmriId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<IsEmriKalemi>()
            .HasOne(k => k.Parca)
            .WithMany()
            .HasForeignKey(k => k.ParcaId)
            .OnDelete(DeleteBehavior.SetNull);

        // Bir iş emrinden en fazla bir fatura kesilir; fatura no kiracı içinde tekil.
        modelBuilder.Entity<Fatura>().HasIndex(f => f.IsEmriId).IsUnique();
        modelBuilder.Entity<Fatura>().HasIndex(f => new { f.TenantId, f.FaturaNo }).IsUnique();
        modelBuilder.Entity<Fatura>()
            .HasOne(f => f.IsEmri).WithMany().HasForeignKey(f => f.IsEmriId)
            .OnDelete(DeleteBehavior.Restrict);

        // Parasal alanların SQLite'ta hassasiyet kaybetmemesi için sabit ölçek.
        modelBuilder.Entity<Parca>().Property(p => p.AlisFiyati).HasColumnType("decimal(18,4)");
        modelBuilder.Entity<Parca>().Property(p => p.SatisFiyati).HasColumnType("decimal(18,4)");
        modelBuilder.Entity<IsEmriKalemi>().Property(k => k.BirimFiyat).HasColumnType("decimal(18,4)");
        modelBuilder.Entity<IsEmriKalemi>().Property(k => k.Adet).HasColumnType("decimal(18,4)");
        modelBuilder.Entity<IsEmriKalemi>().Property(k => k.KdvOrani).HasColumnType("decimal(18,4)");
        modelBuilder.Entity<Fatura>().Property(f => f.AraToplam).HasColumnType("decimal(18,4)");
        modelBuilder.Entity<Fatura>().Property(f => f.ToplamKdv).HasColumnType("decimal(18,4)");
        modelBuilder.Entity<Fatura>().Property(f => f.GenelToplam).HasColumnType("decimal(18,4)");
        modelBuilder.Entity<Gider>().Property(g => g.Tutar).HasColumnType("decimal(18,4)");

        // Muhasebe raporu tarih aralığına göre filtreleniyor — TenantId ile bileşik index.
        modelBuilder.Entity<Gider>().HasIndex(g => new { g.TenantId, g.Tarih });
        modelBuilder.Entity<Fatura>().HasIndex(f => new { f.TenantId, f.Tarih });
    }
}
