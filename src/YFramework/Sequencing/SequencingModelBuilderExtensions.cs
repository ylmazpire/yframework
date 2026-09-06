using Microsoft.EntityFrameworkCore;

namespace YFramework.Sequencing;

public static class SequencingModelBuilderExtensions
{
    /// <summary>
    /// <see cref="SequenceCounter"/> tablosunu modele ekler. Uygulamanın DbContext'inin
    /// <c>OnModelCreating</c> metodunda çağrılır. Ardından <see cref="ISequenceGenerator"/>
    /// (genelde <see cref="EfSequenceGenerator"/>) DI'a kaydedilir.
    /// </summary>
    public static ModelBuilder AddYFrameworkSequences(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SequenceCounter>(e =>
        {
            e.HasKey(x => x.Anahtar);
            e.Property(x => x.Anahtar).HasMaxLength(200);
            e.Property(x => x.SonDeger).IsConcurrencyToken();
        });

        return modelBuilder;
    }
}
