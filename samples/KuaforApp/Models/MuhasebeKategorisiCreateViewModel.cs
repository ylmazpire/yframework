using System.ComponentModel.DataAnnotations;

namespace KuaforApp.Models;

public class MuhasebeKategorisiCreateViewModel
{
    [Required(ErrorMessage = "Kategori adı zorunludur.")]
    public string Ad { get; set; } = string.Empty;

    [Required]
    public IslemTuru Tur { get; set; }
}
