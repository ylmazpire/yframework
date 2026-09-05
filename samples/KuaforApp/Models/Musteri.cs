using System.ComponentModel.DataAnnotations;

namespace KuaforApp.Models;

public class Musteri
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ad soyad zorunludur.")]
    public string AdSoyad { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefon zorunludur.")]
    [Phone(ErrorMessage = "Geçerli bir telefon numarası girin.")]
    public string Telefon { get; set; } = string.Empty;
}
