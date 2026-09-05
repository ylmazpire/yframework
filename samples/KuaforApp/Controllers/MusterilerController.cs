using KuaforApp.Data;
using KuaforApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YFramework.MultiTenancy;

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

    public async Task<IActionResult> Index()
    {
        var musteriler = await _context.Musteriler.OrderBy(m => m.AdSoyad).ToListAsync();
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
            Telefon = model.Telefon
        };

        _context.Musteriler.Add(musteri);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
