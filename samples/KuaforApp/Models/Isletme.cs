using System.ComponentModel.DataAnnotations;

namespace KuaforApp.Models;

public class Isletme
{
    public int Id { get; set; }

    [Required(ErrorMessage = "İşletme adı zorunludur.")]
    public string Ad { get; set; } = string.Empty;

    public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;
}
