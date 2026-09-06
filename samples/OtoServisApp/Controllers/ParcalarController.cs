using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OtoServisApp.Data;
using OtoServisApp.Models;
using YFramework.Inventory;
using YFramework.MultiTenancy;

namespace OtoServisApp.Controllers;

[Authorize(Roles = AppRoles.ServisAdmini)]
public class ParcalarController : Controller
{
    private readonly AppDbContext _context;
    private readonly ICurrentTenantProvider _currentTenant;
    private readonly StockLedger _stok;

    public ParcalarController(AppDbContext context, ICurrentTenantProvider currentTenant, StockLedger stok)
    {
        _context = context;
        _currentTenant = currentTenant;
        _stok = stok;
    }

    public async Task<IActionResult> Hareketler(int id)
    {
        var parca = await _context.Parcalar.FindAsync(id);
        if (parca is null) return NotFound();

        ViewBag.Parca = parca;
        ViewBag.EldekiLedger = await _stok.EldekiMiktarAsync(id);
        return View(await _stok.HareketlerAsync(id));
    }

    public async Task<IActionResult> Index(string? ara)
    {
        var sorgu = _context.Parcalar.AsQueryable();
        if (!string.IsNullOrWhiteSpace(ara))
            sorgu = sorgu.Where(p => p.Ad.Contains(ara) || p.StokKodu.Contains(ara));

        ViewData["Ara"] = ara;
        return View(await sorgu.OrderBy(p => p.Ad).ToListAsync());
    }

    [HttpGet]
    public IActionResult Create() => View(new ParcaFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ParcaFormViewModel vm)
    {
        await ValidateStokKoduTekilligi(vm.StokKodu, null);
        if (!ModelState.IsValid) return View(vm);

        _context.Parcalar.Add(new Parca
        {
            TenantId = _currentTenant.TenantId!.Value,
            StokKodu = vm.StokKodu.Trim(),
            Ad = vm.Ad.Trim(),
            StokAdedi = vm.StokAdedi,
            AlisFiyati = vm.AlisFiyati,
            SatisFiyati = vm.SatisFiyati
        });
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var p = await _context.Parcalar.FindAsync(id);
        if (p is null) return NotFound();
        return View(new ParcaFormViewModel
        {
            Id = p.Id, StokKodu = p.StokKodu, Ad = p.Ad,
            StokAdedi = p.StokAdedi, AlisFiyati = p.AlisFiyati, SatisFiyati = p.SatisFiyati
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ParcaFormViewModel vm)
    {
        await ValidateStokKoduTekilligi(vm.StokKodu, vm.Id);
        if (!ModelState.IsValid) return View(vm);

        var p = await _context.Parcalar.FindAsync(vm.Id);
        if (p is null) return NotFound();

        p.StokKodu = vm.StokKodu.Trim();
        p.Ad = vm.Ad.Trim();
        p.StokAdedi = vm.StokAdedi;
        p.AlisFiyati = vm.AlisFiyati;
        p.SatisFiyati = vm.SatisFiyati;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var p = await _context.Parcalar.FindAsync(id);
        if (p is null) return NotFound();

        if (await _context.IsEmriKalemleri.AnyAsync(k => k.ParcaId == id))
        {
            TempData["Hata"] = $"\"{p.Ad}\" silinemedi çünkü iş emri kalemlerinde kullanılıyor.";
            return RedirectToAction(nameof(Index));
        }

        _context.Parcalar.Remove(p);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidateStokKoduTekilligi(string stokKodu, int? mevcutId)
    {
        var kod = stokKodu.Trim();
        if (await _context.Parcalar.AnyAsync(p => p.StokKodu == kod && (mevcutId == null || p.Id != mevcutId)))
            ModelState.AddModelError(nameof(ParcaFormViewModel.StokKodu), "Bu stok kodu zaten kayıtlı.");
    }
}
