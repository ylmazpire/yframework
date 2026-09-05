using KuaforApp.Data;
using KuaforApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YFramework.MultiTenancy;

namespace KuaforApp.Controllers;

[Authorize(Roles = AppRoles.IsletmeAdmini)]
public class HizmetlerController : Controller
{
    private readonly AppDbContext _context;
    private readonly ICurrentTenantProvider _currentTenant;

    public HizmetlerController(AppDbContext context, ICurrentTenantProvider currentTenant)
    {
        _context = context;
        _currentTenant = currentTenant;
    }

    public async Task<IActionResult> Index()
    {
        var hizmetler = await _context.Hizmetler.OrderBy(h => h.Ad).ToListAsync();
        return View(hizmetler);
    }

    [HttpGet]
    public IActionResult Create() => View(new HizmetCreateViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HizmetCreateViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var hizmet = new Hizmet
        {
            TenantId = _currentTenant.TenantId!.Value,
            Ad = model.Ad,
            SureDakika = model.SureDakika,
            Fiyat = model.Fiyat
        };

        _context.Hizmetler.Add(hizmet);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var hizmet = await _context.Hizmetler.FindAsync(id);
        if (hizmet is null) return NotFound();

        return View(new HizmetEditViewModel
        {
            Id = hizmet.Id,
            Ad = hizmet.Ad,
            SureDakika = hizmet.SureDakika,
            Fiyat = hizmet.Fiyat
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(HizmetEditViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var hizmet = await _context.Hizmetler.FindAsync(model.Id);
        if (hizmet is null) return NotFound();

        hizmet.Ad = model.Ad;
        hizmet.SureDakika = model.SureDakika;
        hizmet.Fiyat = model.Fiyat;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var hizmet = await _context.Hizmetler.FindAsync(id);
        if (hizmet is null) return NotFound();

        var randevusuVarMi = await _context.Randevular.AnyAsync(r => r.HizmetId == id);
        if (randevusuVarMi)
        {
            TempData["Hata"] = $"\"{hizmet.Ad}\" silinemedi çünkü bu hizmete ait randevu kayıtları var.";
            return RedirectToAction(nameof(Index));
        }

        _context.Hizmetler.Remove(hizmet);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
