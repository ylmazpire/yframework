using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OtoServisApp.Data;
using OtoServisApp.Models;
using YFramework.MultiTenancy;
using YFramework.Validation;

namespace OtoServisApp.Controllers;

[Authorize(Roles = AppRoles.ServisAdmini)]
public class AraclarController : Controller
{
    private readonly AppDbContext _context;
    private readonly ICurrentTenantProvider _currentTenant;

    public AraclarController(AppDbContext context, ICurrentTenantProvider currentTenant)
    {
        _context = context;
        _currentTenant = currentTenant;
    }

    public async Task<IActionResult> Index(string? ara)
    {
        var sorgu = _context.Araclar.Include(a => a.Musteri).AsQueryable();

        if (!string.IsNullOrWhiteSpace(ara))
        {
            var plakaAra = TurkishLicensePlateFormatter.Normalize(ara);
            sorgu = sorgu.Where(a =>
                a.Plaka.Contains(plakaAra) ||
                a.Marka.Contains(ara) ||
                a.Model.Contains(ara) ||
                a.Musteri!.AdSoyad.Contains(ara));
        }

        ViewData["Ara"] = ara;
        return View(await sorgu.OrderBy(a => a.Plaka).ToListAsync());
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? musteriId)
    {
        await DoldurMusteriListesi(musteriId);
        return View(new AracCreateViewModel { MusteriId = musteriId ?? 0 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AracCreateViewModel vm)
    {
        await ValidatePlakaTekilligi(vm.Plaka, null);
        if (!ModelState.IsValid)
        {
            await DoldurMusteriListesi(vm.MusteriId);
            return View(vm);
        }

        _context.Araclar.Add(new Arac
        {
            TenantId = _currentTenant.TenantId!.Value,
            MusteriId = vm.MusteriId,
            Plaka = TurkishLicensePlateFormatter.Normalize(vm.Plaka),
            Marka = vm.Marka.Trim(),
            Model = vm.Model.Trim(),
            ModelYili = vm.ModelYili,
            Kilometre = vm.Kilometre
        });
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var arac = await _context.Araclar.FindAsync(id);
        if (arac is null) return NotFound();

        await DoldurMusteriListesi(arac.MusteriId);
        return View(new AracEditViewModel
        {
            Id = arac.Id,
            MusteriId = arac.MusteriId,
            Plaka = TurkishLicensePlateFormatter.Bicimlendir(arac.Plaka),
            Marka = arac.Marka,
            Model = arac.Model,
            ModelYili = arac.ModelYili,
            Kilometre = arac.Kilometre
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(AracEditViewModel vm)
    {
        await ValidatePlakaTekilligi(vm.Plaka, vm.Id);
        if (!ModelState.IsValid)
        {
            await DoldurMusteriListesi(vm.MusteriId);
            return View(vm);
        }

        var arac = await _context.Araclar.FindAsync(vm.Id);
        if (arac is null) return NotFound();

        arac.MusteriId = vm.MusteriId;
        arac.Plaka = TurkishLicensePlateFormatter.Normalize(vm.Plaka);
        arac.Marka = vm.Marka.Trim();
        arac.Model = vm.Model.Trim();
        arac.ModelYili = vm.ModelYili;
        arac.Kilometre = vm.Kilometre;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var arac = await _context.Araclar.FindAsync(id);
        if (arac is null) return NotFound();

        if (await _context.IsEmirleri.AnyAsync(e => e.AracId == id))
        {
            TempData["Hata"] = $"\"{TurkishLicensePlateFormatter.Bicimlendir(arac.Plaka)}\" silinemedi çünkü bu araca ait iş emri kayıtları var.";
            return RedirectToAction(nameof(Index));
        }

        _context.Araclar.Remove(arac);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidatePlakaTekilligi(string plaka, int? mevcutId)
    {
        var normalize = TurkishLicensePlateFormatter.Normalize(plaka);
        var cakisiyor = await _context.Araclar
            .AnyAsync(a => a.Plaka == normalize && (mevcutId == null || a.Id != mevcutId));
        if (cakisiyor)
        {
            ModelState.AddModelError(nameof(AracFormViewModel.Plaka), "Bu plaka zaten kayıtlı.");
        }
    }

    private async Task DoldurMusteriListesi(int? seciliMusteriId)
    {
        var musteriler = await _context.Musteriler.OrderBy(m => m.AdSoyad).ToListAsync();
        ViewBag.Musteriler = new SelectList(musteriler, nameof(Musteri.Id), nameof(Musteri.AdSoyad), seciliMusteriId);
    }
}
