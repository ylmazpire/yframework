using System.ComponentModel.DataAnnotations;
using YFramework.MultiTenancy;

namespace KuaforApp.Models;

public enum IslemTuru
{
    Gelir,
    Gider
}

public class MuhasebeKategorisi : ITenantScoped
{
    public int Id { get; set; }
    public int TenantId { get; set; }

    [Required(ErrorMessage = "Kategori adı zorunludur.")]
    public string Ad { get; set; } = string.Empty;

    [Required]
    public IslemTuru Tur { get; set; }
}
