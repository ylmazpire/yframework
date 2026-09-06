using System.ComponentModel.DataAnnotations;
using YFramework.MultiTenancy;
using YFramework.Validation;

namespace OtoServisApp.Models;

/// <summary>
/// Servise gelen bir araç. KuaforApp'te karşılığı yok — ikinci örnek uygulamanın framework'e
/// getirdiği ilk yeni kavram.
/// </summary>
public class Arac : ITenantScoped
{
    public int Id { get; set; }
    public int TenantId { get; set; }

    [Required(ErrorMessage = "Müşteri seçilmelidir.")]
    public int MusteriId { get; set; }
    public Musteri? Musteri { get; set; }

    /// <summary>
    /// Plaka. Boşluklu/boşluksuz girilebilir, normalize edilerek saklanır (ör. "34ABC123").
    /// </summary>
    [Required(ErrorMessage = "Plaka zorunludur.")]
    [StringLength(15)]
    [TurkishLicensePlate]
    public string Plaka { get; set; } = string.Empty;

    [Required(ErrorMessage = "Marka zorunludur.")]
    [StringLength(50)]
    public string Marka { get; set; } = string.Empty;

    [Required(ErrorMessage = "Model zorunludur.")]
    [StringLength(50)]
    public string Model { get; set; } = string.Empty;

    [Range(1950, 2100, ErrorMessage = "Geçerli bir model yılı girin.")]
    public int ModelYili { get; set; }

    [Range(0, 5_000_000, ErrorMessage = "Geçerli bir kilometre girin.")]
    public int Kilometre { get; set; }

    public List<IsEmri> IsEmirleri { get; set; } = new();
}
