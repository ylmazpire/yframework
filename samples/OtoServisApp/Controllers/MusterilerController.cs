using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OtoServisApp.Data;
using OtoServisApp.Models;
using YFramework.MultiTenancy;
using YFramework.Validation;

namespace OtoServisApp.Controllers;

[Authorize(Roles = AppRoles.ServisAdmini)]
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
            sorgu = sorgu.Where(m => m.AdSoyad.Contains(ara) || m.Telefon.Contains(ara) || m.VergiKimlikNo!.Contains(ara));
        }

        ViewData["Ara"] = ara;
        return View(await sorgu.OrderBy(m => m.AdSoyad).ToListAsync());
    }

    [HttpGet]
    public IActionResult Create() => View(new MusteriCreateViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MusteriCreateViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        _context.Musteriler.Add(new Musteri
        {
            TenantId = _currentTenant.TenantId!.Value,
            AdSoyad = model.AdSoyad.Trim(),
            Telefon = TurkishPhoneNumberFormatter.Normalize(model.Telefon),
            VergiKimlikNo = string.IsNullOrWhiteSpace(model.VergiKimlikNo) ? null : model.VergiKimlikNo.Trim()
        });
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Detay(int id)
    {
        var musteri = await _context.Musteriler.FindAsync(id);
        if (musteri is null) return NotFound();

        var araclar = await _context.Araclar.Where(a => a.MusteriId == id).OrderBy(a => a.Plaka).ToListAsync();
        var isEmriSayisi = await _context.IsEmirleri.CountAsync(e => e.Arac!.MusteriId == id);

        return View(new MusteriDetayViewModel
        {
            Musteri = musteri,
            Araclar = araclar,
            ToplamIsEmriSayisi = isEmriSayisi
        });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var musteri = await _context.Musteriler.FindAsync(id);
        if (musteri is null) return NotFound();

        return View(new MusteriEditViewModel
        {
            Id = musteri.Id,
            AdSoyad = musteri.AdSoyad,
            Telefon = musteri.Telefon,
            VergiKimlikNo = musteri.VergiKimlikNo
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(MusteriEditViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var musteri = await _context.Musteriler.FindAsync(model.Id);
        if (musteri is null) return NotFound();

        musteri.AdSoyad = model.AdSoyad.Trim();
        musteri.Telefon = TurkishPhoneNumberFormatter.Normalize(model.Telefon);
        musteri.VergiKimlikNo = string.IsNullOrWhiteSpace(model.VergiKimlikNo) ? null : model.VergiKimlikNo.Trim();
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var musteri = await _context.Musteriler.FindAsync(id);
        if (musteri is null) return NotFound();

        if (await _context.Araclar.AnyAsync(a => a.MusteriId == id))
        {
            TempData["Hata"] = $"\"{musteri.AdSoyad}\" silinemedi çünkü bu müşteriye kayıtlı araç var.";
            return RedirectToAction(nameof(Index));
        }

        _context.Musteriler.Remove(musteri);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
