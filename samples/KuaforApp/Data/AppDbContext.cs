using KuaforApp.Models;
using Microsoft.EntityFrameworkCore;
using YFramework.Auth;

namespace KuaforApp.Data;

public class AppDbContext : YFrameworkIdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Musteri> Musteriler => Set<Musteri>();
}
