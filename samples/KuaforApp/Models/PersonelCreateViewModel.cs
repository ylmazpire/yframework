using System.ComponentModel.DataAnnotations;

namespace KuaforApp.Models;

public class PersonelCreateViewModel
{
    [Required(ErrorMessage = "Ad soyad zorunludur.")]
    [StringLength(200)]
    public string AdSoyad { get; set; } = string.Empty;
}
