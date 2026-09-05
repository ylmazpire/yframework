using Microsoft.EntityFrameworkCore;

namespace YFramework.Data;

/// <summary>
/// Uygulamaların kendi DbContext'lerinin türeteceği temel sınıf.
/// </summary>
public abstract class YFrameworkDbContext : DbContext
{
    protected YFrameworkDbContext(DbContextOptions options) : base(options)
    {
    }
}
