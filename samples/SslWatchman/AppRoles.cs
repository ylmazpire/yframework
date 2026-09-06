using YFramework.Auth;

namespace SslWatchman;

/// <summary>
/// Bu uygulamaya özgü roller. YFramework.Auth.Roles.Admin (platform yöneticisi) framework'ten gelir;
/// Yonetici ise SslWatchman'e özgü — bir hesabın (kiracının) domainlerini yöneten kullanıcı.
/// </summary>
public static class AppRoles
{
    public const string Yonetici = "Yonetici";

    public static readonly string[] All = { Roles.Admin, Yonetici };
}
