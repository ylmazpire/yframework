using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OtoServisApp.Data;
using OtoServisApp.Models;
using YFramework.MultiTenancy;
using YFramework.Validation;

namespace OtoServisApp.Controllers;

[Authorize(Roles = AppRoles.ServisAdmini)]
public class PersonelController : Controller
{
    private readonly AppDbContext _context;
    private readonly ICurrentTenantProvider _currentTenant;

    public PersonelController(AppDbContext context, ICurrentTenantProvider currentTenant)
    {
        _context = context;
        _currentTenant = currentTenant;
    }

    public async Task<IActionResult> Index()
        => View(await _context.Personeller.OrderBy(p => p.AdSoyad).ToListAsync());

    [HttpGet]
    public IActionResult Create() => View(new PersonelFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PersonelFormViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        _context.Personeller.Add(new Personel
        {
            TenantId = _currentTenant.TenantId!.Value,
            AdSoyad = vm.AdSoyad.Trim(),
            Telefon = string.IsNullOrWhiteSpace(vm.Telefon) ? null : TurkishPhoneNumberFormatter.Normalize(vm.Telefon),
            Uzmanlik = string.IsNullOrWhiteSpace(vm.Uzmanlik) ? null : vm.Uzmanlik.Trim(),
            Aktif = vm.Aktif
        });
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var p = await _context.Personeller.FindAsync(id);
        if (p is null) return NotFound();
        return View(new PersonelFormViewModel
        {
            Id = p.Id, AdSoyad = p.AdSoyad, Telefon = p.Telefon, Uzmanlik = p.Uzmanlik, Aktif = p.Aktif
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(PersonelFormViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var p = await _context.Personeller.FindAsync(vm.Id);
        if (p is null) return NotFound();

        p.AdSoyad = vm.AdSoyad.Trim();
        p.Telefon = string.IsNullOrWhiteSpace(vm.Telefon) ? null : TurkishPhoneNumberFormatter.Normalize(vm.Telefon);
        p.Uzmanlik = string.IsNullOrWhiteSpace(vm.Uzmanlik) ? null : vm.Uzmanlik.Trim();
        p.Aktif = vm.Aktif;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var p = await _context.Personeller.FindAsync(id);
        if (p is null) return NotFound();

        if (await _context.IsEmirleri.AnyAsync(e => e.AtananPersonelId == id))
        {
            TempData["Hata"] = $"\"{p.AdSoyad}\" silinemedi çünkü bu ustaya atanmış iş emirleri var. Bunun yerine pasif yapabilirsin.";
            return RedirectToAction(nameof(Index));
        }

        _context.Personeller.Remove(p);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
