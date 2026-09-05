namespace YFramework.MultiTenancy;

/// <summary>
/// O anki isteği yapan kullanıcının hangi kiracıya (işletmeye) ait olduğunu sağlar.
/// null dönerse (örn. platform admini), kiracı filtresi uygulanmaz — tüm kayıtlar görünür.
/// </summary>
public interface ICurrentTenantProvider
{
    int? TenantId { get; }
}
