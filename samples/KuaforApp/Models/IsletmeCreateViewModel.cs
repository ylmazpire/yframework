using System.ComponentModel.DataAnnotations;

namespace KuaforApp.Models;

public class IsletmeCreateViewModel
{
    [Required(ErrorMessage = "İşletme adı zorunludur.")]
    [StringLength(200)]
    public string IsletmeAdi { get; set; } = string.Empty;

    [Required(ErrorMessage = "Yetkili adı soyadı zorunludur.")]
    [StringLength(200)]
    public string YetkiliAdSoyad { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress]
    public string YetkiliEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalı.")]
    public string YetkiliSifre { get; set; } = string.Empty;
}
