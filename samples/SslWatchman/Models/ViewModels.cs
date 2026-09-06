using System.ComponentModel.DataAnnotations;
using YFramework.Monitoring;

namespace SslWatchman.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}

public class DomainFormViewModel
{
    public int Id { get; set; }

    [Display(Name = "Alan adı")]
    [Required(ErrorMessage = "Alan adı zorunludur.")]
    [StringLength(253)]
    public string Host { get; set; } = string.Empty;

    [Display(Name = "Port")]
    [Range(1, 65535, ErrorMessage = "Geçerli bir port girin.")]
    public int Port { get; set; } = 443;

    [Display(Name = "Aktif")]
    public bool Aktif { get; set; } = true;

    [Display(Name = "Not")]
    [StringLength(300)]
    public string? Not { get; set; }
}

public class AyarlarViewModel
{
    [Display(Name = "Uyarı webhook URL'si")]
    [StringLength(500)]
    [Url(ErrorMessage = "Geçerli bir URL girin.")]
    public string? WebhookUrl { get; set; }

    [Display(Name = "Kaç gün kala uyar")]
    [Range(1, 90, ErrorMessage = "1 ile 90 arasında olmalı.")]
    public int UyariEsigiGun { get; set; } = 21;
}

public class DomainDetayViewModel
{
    public IzlenenDomain Domain { get; set; } = null!;
    public List<MonitorCheck> Gecmis { get; set; } = new();
}

public class PanoViewModel
{
    public List<IzlenenDomain> Domainler { get; set; } = new();

    public int Toplam => Domainler.Count;
    public int Kritik => Domainler.Count(d => d.SonSonuc is CheckOutcome.Critical or CheckOutcome.Error);
    public int Uyari => Domainler.Count(d => d.SonSonuc == CheckOutcome.Warning);
    public int Saglikli => Domainler.Count(d => d.SonSonuc == CheckOutcome.Ok);
    public int Kontrolsuz => Domainler.Count(d => d.SonSonuc is null);
}
