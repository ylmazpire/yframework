using KuaforApp.Data;
using KuaforApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YFramework.MultiTenancy;
using YFramework.Validation;

namespace KuaforApp.Controllers;

[Authorize(Roles = AppRoles.IsletmeAdmini)]
public class MusterilerController : Controller
{
    private readonly AppDbContext _context;
    private readonly ICurrentTenantProvider _currentTenant;

    public MusterilerController(AppDbContext context, ICurrentTenantProvider currentTenant)
    {
        _context = context;
        _currentTenant = currentTenant;
    }

    public async Task<IActionResult> Index(string? ara)
    {
        var sorgu = _context.Musteriler.AsQueryable();

        if (!string.IsNullOrWhiteSpace(ara))
        {
            sorgu = sorgu.Where(m => m.AdSoyad.Contains(ara) || m.Telefon.Contains(ara));
        }

        ViewData["Ara"] = ara;
        var musteriler = await sorgu.OrderBy(m => m.AdSoyad).ToListAsync();
        return View(musteriler);
    }

    [HttpGet]
    public IActionResult Create() => View(new MusteriCreateViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MusteriCreateViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var musteri = new Musteri
        {
            TenantId = _currentTenant.TenantId!.Value,
            AdSoyad = model.AdSoyad,
            Telefon = TurkishPhoneNumberFormatter.Normalize(model.Telefon)
        };

        _context.Musteriler.Add(musteri);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Detay(int id)
    {
        var musteri = await _context.Musteriler.FindAsync(id);
        if (musteri is null) return NotFound();

        var randevular = await _context.Randevular
            .Include(r => r.Hizmet)
            .Include(r => r.Personel)
            .Where(r => r.MusteriId == id)
            .OrderByDescending(r => r.BaslangicZamani)
            .ToListAsync();

        var tamamlananlar = randevular.Where(r => r.Durum == RandevuDurumu.Tamamlandi).ToList();

        return View(new MusteriDetayViewModel
        {
            Musteri = musteri,
            Randevular = randevular,
            TamamlananRandevuSayisi = tamamlananlar.Count,
            ToplamHarcama = tamamlananlar.Sum(r => r.Hizmet?.Fiyat ?? 0)
        });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var musteri = await _context.Musteriler.FindAsync(id);
        if (musteri is null) return NotFound();

        return View(new MusteriEditViewModel { Id = musteri.Id, AdSoyad = musteri.AdSoyad, Telefon = musteri.Telefon });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(MusteriEditViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var musteri = await _context.Musteriler.FindAsync(model.Id);
        if (musteri is null) return NotFound();

        musteri.AdSoyad = model.AdSoyad;
        musteri.Telefon = TurkishPhoneNumberFormatter.Normalize(model.Telefon);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var musteri = await _context.Musteriler.FindAsync(id);
        if (musteri is null) return NotFound();

        var randevusuVarMi = await _context.Randevular.AnyAsync(r => r.MusteriId == id);
        if (randevusuVarMi)
        {
            TempData["Hata"] = $"\"{musteri.AdSoyad}\" silinemedi çünkü bu müşteriye ait randevu kayıtları var.";
            return RedirectToAction(nameof(Index));
        }

        _context.Musteriler.Remove(musteri);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
