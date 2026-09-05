using System.ComponentModel.DataAnnotations;

namespace KuaforApp.Models;

public enum IslemTuru
{
    Gelir,
    Gider
}

public class MuhasebeKategorisi
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Kategori adı zorunludur.")]
    public string Ad { get; set; } = string.Empty;

    [Required]
    public IslemTuru Tur { get; set; }
}
