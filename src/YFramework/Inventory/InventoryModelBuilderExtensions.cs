using Microsoft.EntityFrameworkCore;

namespace YFramework.Inventory;

public static class InventoryModelBuilderExtensions
{
    /// <summary>
    /// <see cref="StockMovement"/> tablosunu modele ekler. Uygulamanın DbContext'inin
    /// <c>OnModelCreating</c> metodunda çağrılır. Ardından <see cref="StockLedger"/> DI'a kaydedilir.
    /// </summary>
    public static ModelBuilder AddYFrameworkStockLedger(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StockMovement>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Miktar).HasColumnType("decimal(18,4)");
            e.Property(x => x.Sebep).HasMaxLength(200);
            e.Property(x => x.Referans).HasMaxLength(100);
            // Eldeki miktar sorgusu hep tek kalem için toplam alır.
            e.HasIndex(x => x.StokKalemId);
        });

        return modelBuilder;
    }
}
