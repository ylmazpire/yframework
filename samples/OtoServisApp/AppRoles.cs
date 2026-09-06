using YFramework.Auth;

namespace OtoServisApp;

/// <summary>
/// Bu uygulamaya özgü roller. YFramework.Auth.Roles.Admin (platform yöneticisi) framework'ten gelir;
/// ServisAdmini ise OtoServisApp'e özgü bir roldür (tek bir servisin/işletmenin sahibi/yöneticisi).
/// </summary>
public static class AppRoles
{
    public const string ServisAdmini = "ServisAdmini";

    public static readonly string[] All = { Roles.Admin, ServisAdmini };
}
