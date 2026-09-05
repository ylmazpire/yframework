using KuaforApp.Data;
using KuaforApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YFramework.MultiTenancy;

namespace KuaforApp.Controllers;

[Authorize(Roles = AppRoles.IsletmeAdmini)]
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
    {
        var personeller = await _context.Personeller.OrderBy(p => p.AdSoyad).ToListAsync();
        return View(personeller);
    }

    [HttpGet]
    public IActionResult Create() => View(new PersonelCreateViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PersonelCreateViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var personel = new Personel
        {
            TenantId = _currentTenant.TenantId!.Value,
            AdSoyad = model.AdSoyad
        };

        _context.Personeller.Add(personel);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var personel = await _context.Personeller.FindAsync(id);
        if (personel is null) return NotFound();

        return View(new PersonelEditViewModel { Id = personel.Id, AdSoyad = personel.AdSoyad });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(PersonelEditViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var personel = await _context.Personeller.FindAsync(model.Id);
        if (personel is null) return NotFound();

        personel.AdSoyad = model.AdSoyad;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var personel = await _context.Personeller.FindAsync(id);
        if (personel is null) return NotFound();

        var randevusuVarMi = await _context.Randevular.AnyAsync(r => r.PersonelId == id);
        if (randevusuVarMi)
        {
            TempData["Hata"] = $"\"{personel.AdSoyad}\" silinemedi çünkü bu personele ait randevu kayıtları var.";
            return RedirectToAction(nameof(Index));
        }

        _context.Personeller.Remove(personel);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
