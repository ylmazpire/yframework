using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OtoServisApp.Data;
using OtoServisApp.Models;
using YFramework.MultiTenancy;

namespace OtoServisApp.Controllers;

[Authorize(Roles = AppRoles.ServisAdmini)]
public class ServisKanallariController : Controller
{
    private readonly AppDbContext _context;
    private readonly ICurrentTenantProvider _currentTenant;

    public ServisKanallariController(AppDbContext context, ICurrentTenantProvider currentTenant)
    {
        _context = context;
        _currentTenant = currentTenant;
    }

    public async Task<IActionResult> Index()
        => View(await _context.ServisKanallari.OrderBy(k => k.Ad).ToListAsync());

    [HttpGet]
    public IActionResult Create() => View(new ServisKanaliFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ServisKanaliFormViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        _context.ServisKanallari.Add(new ServisKanali
        {
            TenantId = _currentTenant.TenantId!.Value,
            Ad = vm.Ad.Trim(),
            Aktif = vm.Aktif
        });
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var kanal = await _context.ServisKanallari.FindAsync(id);
        if (kanal is null) return NotFound();
        return View(new ServisKanaliFormViewModel { Id = kanal.Id, Ad = kanal.Ad, Aktif = kanal.Aktif });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ServisKanaliFormViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var kanal = await _context.ServisKanallari.FindAsync(vm.Id);
        if (kanal is null) return NotFound();

        kanal.Ad = vm.Ad.Trim();
        kanal.Aktif = vm.Aktif;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var kanal = await _context.ServisKanallari.FindAsync(id);
        if (kanal is null) return NotFound();

        if (await _context.IsEmirleri.AnyAsync(e => e.ServisKanaliId == id))
        {
            TempData["Hata"] = $"\"{kanal.Ad}\" silinemedi çünkü bu kanala planlanmış iş emirleri var.";
            return RedirectToAction(nameof(Index));
        }

        _context.ServisKanallari.Remove(kanal);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
