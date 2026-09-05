using YFramework.Auth;

namespace KuaforApp;

/// <summary>
/// Bu uygulamaya özgü roller. YFramework.Auth.Roles.Admin (platform yöneticisi) framework'ten gelir;
/// İşletmeAdmini ise KuaforApp'e özgü bir roldür.
/// </summary>
public static class AppRoles
{
    public const string IsletmeAdmini = "IsletmeAdmini";

    public static readonly string[] All = { Roles.Admin, IsletmeAdmini };
}
