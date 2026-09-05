using System.ComponentModel.DataAnnotations;

namespace KuaforApp.Models;

public class PersonelEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ad soyad zorunludur.")]
    public string AdSoyad { get; set; } = string.Empty;
}
