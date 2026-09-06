using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OtoServisApp.Data;
using OtoServisApp.Models;
using YFramework.MultiTenancy;

namespace OtoServisApp.Controllers;

[Authorize(Roles = AppRoles.ServisAdmini)]
public class GiderlerController : Controller
{
    private readonly AppDbContext _context;
    private readonly ICurrentTenantProvider _currentTenant;

    public GiderlerController(AppDbContext context, ICurrentTenantProvider currentTenant)
    {
        _context = context;
        _currentTenant = currentTenant;
    }

    public async Task<IActionResult> Index()
        => View(await _context.Giderler.OrderByDescending(g => g.Tarih).ToListAsync());

    [HttpGet]
    public IActionResult Create() => View(new GiderFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GiderFormViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        _context.Giderler.Add(new Gider
        {
            TenantId = _currentTenant.TenantId!.Value,
            Aciklama = vm.Aciklama.Trim(),
            Tutar = vm.Tutar,
            Tarih = vm.Tarih,
            Kategori = vm.Kategori
        });
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var g = await _context.Giderler.FindAsync(id);
        if (g is null) return NotFound();
        return View(new GiderFormViewModel
        {
            Id = g.Id, Aciklama = g.Aciklama, Tutar = g.Tutar, Tarih = g.Tarih, Kategori = g.Kategori
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(GiderFormViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var g = await _context.Giderler.FindAsync(vm.Id);
        if (g is null) return NotFound();

        g.Aciklama = vm.Aciklama.Trim();
        g.Tutar = vm.Tutar;
        g.Tarih = vm.Tarih;
        g.Kategori = vm.Kategori;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var g = await _context.Giderler.FindAsync(id);
        if (g is null) return NotFound();
        _context.Giderler.Remove(g);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
