using System.ComponentModel.DataAnnotations;

namespace KuaforApp.Models;

public class SifreSifirlaViewModel
{
    public string KullaniciId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Yeni şifre zorunludur.")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalı.")]
    public string YeniSifre { get; set; } = string.Empty;
}
