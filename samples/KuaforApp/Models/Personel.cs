using System.ComponentModel.DataAnnotations;
using YFramework.MultiTenancy;

namespace KuaforApp.Models;

public class Personel : ITenantScoped
{
    public int Id { get; set; }
    public int TenantId { get; set; }

    [Required(ErrorMessage = "Ad soyad zorunludur.")]
    [StringLength(200)]
    public string AdSoyad { get; set; } = string.Empty;
}
