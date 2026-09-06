using System.ComponentModel.DataAnnotations;

namespace OtoServisApp.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
