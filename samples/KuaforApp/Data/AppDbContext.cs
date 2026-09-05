using KuaforApp.Models;
using Microsoft.EntityFrameworkCore;
using YFramework.Data;

namespace KuaforApp.Data;

public class AppDbContext : YFrameworkDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Musteri> Musteriler => Set<Musteri>();
}
