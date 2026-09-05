using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace YFramework.MultiTenancy;

/// <summary>
/// ITenantScoped implemente eden tüm entity'lere otomatik olarak kiracı filtresi uygular.
/// AppDbContext.OnModelCreating içinde bir kez çağrılır; sonrasında her sorgu otomatik olarak
/// mevcut kullanıcının kiracısına göre filtrelenir — geliştiricinin her sorguya "Where(TenantId == ...)"
/// eklemesi gerekmez.
/// </summary>
public static class TenantQueryFilterApplier
{
    public static void Apply(ModelBuilder modelBuilder, Func<int?> currentTenantIdAccessor)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(ITenantScoped).IsAssignableFrom(entityType.ClrType)) continue;

            var method = typeof(TenantQueryFilterApplier)
                .GetMethod(nameof(SetQueryFilter), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(entityType.ClrType);

            method.Invoke(null, new object[] { modelBuilder, currentTenantIdAccessor });

            // TenantId, otomatik filtre sayesinde HER sorguda WHERE koşuluna giriyor —
            // bu yüzden index'lenmemesi, veri büyüdükçe her isteği yavaşlatacak en kritik
            // eksiklik olurdu. Geliştiricinin bunu hatırlamasına gerek kalmasın diye
            // framework burada otomatik olarak ekliyor.
            modelBuilder.Entity(entityType.ClrType).HasIndex(nameof(ITenantScoped.TenantId));
        }
    }

    private static void SetQueryFilter<TEntity>(ModelBuilder modelBuilder, Func<int?> currentTenantIdAccessor)
        where TEntity : class, ITenantScoped
    {
        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(entity => currentTenantIdAccessor() == null || entity.TenantId == currentTenantIdAccessor());
    }
}
