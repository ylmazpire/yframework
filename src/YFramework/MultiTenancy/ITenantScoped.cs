namespace YFramework.MultiTenancy;

/// <summary>
/// Bir kaydın hangi kiracıya (işletmeye) ait olduğunu belirtir.
/// Bu arayüzü implemente eden her entity, AppDbContext tarafından otomatik olarak
/// mevcut kullanıcının kiracısına göre filtrelenir (bkz. TenantQueryFilterApplier).
/// </summary>
public interface ITenantScoped
{
    int TenantId { get; set; }
}
