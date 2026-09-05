using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace YFramework.Auth;

/// <summary>
/// Kimlik doğrulama (Identity) tablolarını içeren, uygulamaların kendi DbContext'lerinin
/// türeteceği temel sınıf. Roller string olarak (IdentityRole) tutulur.
/// </summary>
public abstract class YFrameworkIdentityDbContext<TUser> : IdentityDbContext<TUser>
    where TUser : IdentityUser
{
    protected YFrameworkIdentityDbContext(DbContextOptions options) : base(options)
    {
    }
}
