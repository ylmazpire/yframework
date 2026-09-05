using YFramework.Auth;

namespace KuaforApp.Models;

public class IsletmeDetayViewModel
{
    public Isletme Isletme { get; set; } = null!;
    public List<ApplicationUser> Kullanicilar { get; set; } = new();
}
