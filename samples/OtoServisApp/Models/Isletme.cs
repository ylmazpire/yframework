using System.ComponentModel.DataAnnotations;

namespace OtoServisApp.Models;

/// <summary>
/// Kiracının kendisi (bir oto servis işletmesi). Bu tablo <see cref="YFramework.MultiTenancy.ITenantScoped"/>
/// değildir — kiracıyı O tanımlar.
/// </summary>
public class Isletme
{
    public int Id { get; set; }

    [Required(ErrorMessage = "İşletme adı zorunludur.")]
    [StringLength(100)]
    public string Ad { get; set; } = string.Empty;

    public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;
}
