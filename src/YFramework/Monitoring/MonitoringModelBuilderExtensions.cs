using Microsoft.EntityFrameworkCore;

namespace YFramework.Monitoring;

public static class MonitoringModelBuilderExtensions
{
    /// <summary>
    /// <see cref="MonitorCheck"/> tablosunu modele ekler. Uygulamanın DbContext'inin
    /// <c>OnModelCreating</c> metodunda çağrılır.
    /// </summary>
    public static ModelBuilder AddYFrameworkMonitoring(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MonitorCheck>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Mesaj).HasMaxLength(500);
            // Panodaki sorgular hep "bir hedef için, zamana göre sıralı".
            e.HasIndex(x => new { x.TargetId, x.ZamanUtc });
        });

        return modelBuilder;
    }
}
