using KuaforApp.Data;
using KuaforApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using YFramework.MultiTenancy;
using YFramework.Reporting;

namespace KuaforApp.Controllers;

[Authorize(Roles = AppRoles.IsletmeAdmini)]
public class MuhasebeController : Controller
{
    private readonly AppDbContext _context;
    private readonly ICurrentTenantProvider _currentTenant;

    public MuhasebeController(AppDbContext context, ICurrentTenantProvider currentTenant)
    {
        _context = context;
        _currentTenant = currentTenant;
    }

    public async Task<IActionResult> Index(int? yil, int? ay)
    {
        var simdi = DateTime.Today;
        var seciliYil = Math.Clamp(yil ?? simdi.Year, 2000, 2100);
        var seciliAy = Math.Clamp(ay ?? simdi.Month, 1, 12);

        var kayitlar = await _context.MuhasebeKayitlari
            .Include(k => k.Kategori)
            .Where(k => k.Tarih.Year == seciliYil && k.Tarih.Month == seciliAy)
            .OrderByDescending(k => k.Tarih)
            .ToListAsync();

        var model = new MuhasebeRaporViewModel
        {
            Yil = seciliYil,
            Ay = seciliAy,
            Ozet = FinancialSummaryCalculator.Ozet(kayitlar),
            Kayitlar = kayitlar
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Export(int yil, int ay)
    {
        var kayitlar = await _context.MuhasebeKayitlari
            .Include(k => k.Kategori)
            .Where(k => k.Tarih.Year == yil && k.Tarih.Month == ay)
            .OrderBy(k => k.Tarih)
            .ToListAsync();

        var basliklar = new[] { "Tarih", "Tür", "Kategori", "Açıklama", "Tutar" };
        var satirlar = kayitlar.Select(k => new[]
        {
            k.Tarih.ToString("dd.MM.yyyy"),
            k.GelirMi ? "Gelir" : "Gider",
            k.Kategori?.Ad ?? "",
            k.Aciklama,
            k.Tutar.ToString("F2")
        });

        var csv = CsvExporter.ToCsv(basliklar, satirlar);
        var dosyaAdi = $"muhasebe_{yil}_{ay:D2}.csv";
        return File(csv, "text/csv", dosyaAdi);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        return View(await BuildViewModelAsync(new MuhasebeCreateViewModel()));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MuhasebeCreateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(await BuildViewModelAsync(model));

        var kayit = new MuhasebeKaydi
        {
            TenantId = _currentTenant.TenantId!.Value,
            KategoriId = model.KategoriId,
            Tutar = model.Tutar,
            Aciklama = model.Aciklama,
            Tarih = model.Tarih
        };

        _context.MuhasebeKayitlari.Add(kayit);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { yil = model.Tarih.Year, ay = model.Tarih.Month });
    }

    private async Task<MuhasebeCreateViewModel> BuildViewModelAsync(MuhasebeCreateViewModel model)
    {
        model.Kategoriler = await _context.MuhasebeKategorileri
            .OrderBy(k => k.Tur).ThenBy(k => k.Ad)
            .Select(k => new SelectListItem
            {
                Value = k.Id.ToString(),
                Text = (k.Tur == IslemTuru.Gelir ? "Gelir: " : "Gider: ") + k.Ad
            })
            .ToListAsync();
        return model;
    }
}
