using Microsoft.EntityFrameworkCore;

namespace YFramework.Sequencing;

/// <summary>
/// <see cref="ISequenceGenerator"/>'ın EF Core ile çalışan uygulaması. Sayaçları
/// <see cref="SequenceCounter"/> tablosunda tutar. Uygulamanın DbContext'i bu entity'yi
/// tanımalıdır — bkz. <see cref="SequencingModelBuilderExtensions.AddYFrameworkSequences"/>.
///
/// Eşzamanlılık: <see cref="SequenceCounter.SonDeger"/> bir eşzamanlılık belirteci olduğu için,
/// iki istek aynı anda artırırsa biri <see cref="DbUpdateConcurrencyException"/> alır; bu durumda
/// birkaç kez yeniden denenir. Böylece numaralar boşluksuz ve tekil kalır.
/// </summary>
public class EfSequenceGenerator : ISequenceGenerator
{
    private const int MaxDeneme = 5;

    private readonly DbContext _db;

    public EfSequenceGenerator(DbContext db)
    {
        _db = db;
    }

    public async Task<long> NextAsync(string anahtar, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(anahtar))
            throw new ArgumentException("Sıra anahtarı boş olamaz.", nameof(anahtar));

        for (var deneme = 1; ; deneme++)
        {
            var sayac = await _db.Set<SequenceCounter>()
                .FirstOrDefaultAsync(c => c.Anahtar == anahtar, cancellationToken);

            if (sayac is null)
            {
                sayac = new SequenceCounter { Anahtar = anahtar, SonDeger = 0 };
                _db.Add(sayac);
            }

            sayac.SonDeger++;

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                return sayac.SonDeger;
            }
            catch (DbUpdateException) when (deneme < MaxDeneme)
            {
                // Eşzamanlı ekleme (aynı anahtarı ilk kez üreten iki istek) ya da eşzamanlılık
                // çakışması: entity'yi takipten çıkar, taze oku, tekrar dene.
                foreach (var entry in _db.ChangeTracker.Entries<SequenceCounter>().ToList())
                {
                    await entry.ReloadAsync(cancellationToken);
                    if (entry.State == EntityState.Detached) continue;
                    entry.State = EntityState.Detached;
                }
            }
        }
    }
}
