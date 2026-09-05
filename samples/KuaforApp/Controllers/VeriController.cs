using KuaforApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YFramework.Reporting;

namespace KuaforApp.Controllers;

/// <summary>
/// İşletme admininin kendi verisini CSV olarak indirebildiği basit bir yedekleme ekranı.
/// </summary>
[Authorize(Roles = AppRoles.IsletmeAdmini)]
public class VeriController : Controller
{
    private readonly AppDbContext _context;

    public VeriController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index() => View();

    [HttpGet]
    public async Task<IActionResult> Musteriler()
    {
        var musteriler = await _context.Musteriler.OrderBy(m => m.AdSoyad).ToListAsync();
        var csv = CsvExporter.ToCsv(
            new[] { "AdSoyad", "Telefon" },
            musteriler.Select(m => new[] { m.AdSoyad, m.Telefon }));
        return File(csv, "text/csv", "musteriler.csv");
    }

    [HttpGet]
    public async Task<IActionResult> Randevular()
    {
        var randevular = await _context.Randevular
            .Include(r => r.Musteri)
            .Include(r => r.Hizmet)
            .Include(r => r.Personel)
            .OrderBy(r => r.BaslangicZamani)
            .ToListAsync();

        var csv = CsvExporter.ToCsv(
            new[] { "Tarih", "Musteri", "Hizmet", "Personel", "Durum", "Not" },
            randevular.Select(r => new[]
            {
                r.BaslangicZamani.ToString("dd.MM.yyyy HH:mm"),
                r.Musteri?.AdSoyad ?? "",
                r.Hizmet?.Ad ?? "",
                r.Personel?.AdSoyad ?? "",
                r.Durum.ToString(),
                r.Not ?? ""
            }));
        return File(csv, "text/csv", "randevular.csv");
    }

    [HttpGet]
    public async Task<IActionResult> Muhasebe()
    {
        var kayitlar = await _context.MuhasebeKayitlari
            .Include(k => k.Kategori)
            .OrderBy(k => k.Tarih)
            .ToListAsync();

        var csv = CsvExporter.ToCsv(
            new[] { "Tarih", "Tur", "Kategori", "Aciklama", "Tutar" },
            kayitlar.Select(k => new[]
            {
                k.Tarih.ToString("dd.MM.yyyy"),
                k.GelirMi ? "Gelir" : "Gider",
                k.Kategori?.Ad ?? "",
                k.Aciklama,
                k.Tutar.ToString("F2")
            }));
        return File(csv, "text/csv", "muhasebe.csv");
    }

    [HttpGet]
    public async Task<IActionResult> Hizmetler()
    {
        var hizmetler = await _context.Hizmetler.OrderBy(h => h.Ad).ToListAsync();
        var csv = CsvExporter.ToCsv(
            new[] { "Ad", "SureDakika", "Fiyat" },
            hizmetler.Select(h => new[] { h.Ad, h.SureDakika.ToString(), h.Fiyat.ToString("F2") }));
        return File(csv, "text/csv", "hizmetler.csv");
    }
}
