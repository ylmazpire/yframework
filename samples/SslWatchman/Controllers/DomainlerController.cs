using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SslWatchman.Data;
using SslWatchman.Models;
using SslWatchman.Services;
using YFramework.MultiTenancy;

namespace SslWatchman.Controllers;

[Authorize(Roles = AppRoles.Yonetici)]
public class DomainlerController : Controller
{
    private readonly AppDbContext _context;
    private readonly ICurrentTenantProvider _currentTenant;
    private readonly DomainKontrolOrkestratoru _orkestratör;

    public DomainlerController(AppDbContext context, ICurrentTenantProvider currentTenant, DomainKontrolOrkestratoru orkestratör)
    {
        _context = context;
        _currentTenant = currentTenant;
        _orkestratör = orkestratör;
    }

    public async Task<IActionResult> Index()
        => View(await _context.IzlenenDomainler.OrderBy(d => d.Host).ToListAsync());

    [HttpGet]
    public IActionResult Create() => View(new DomainFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DomainFormViewModel vm)
    {
        var host = Normalize(vm.Host);
        if (host.Length == 0)
            ModelState.AddModelError(nameof(vm.Host), "Alan adı geçersiz.");
        else if (await _context.IzlenenDomainler.AnyAsync(d => d.Host == host && d.Port == vm.Port))
            ModelState.AddModelError(nameof(vm.Host), "Bu alan adı + port zaten ekli.");

        if (!ModelState.IsValid) return View(vm);

        var domain = new IzlenenDomain
        {
            TenantId = _currentTenant.TenantId!.Value,
            Host = host,
            Port = vm.Port,
            Aktif = vm.Aktif,
            Not = string.IsNullOrWhiteSpace(vm.Not) ? null : vm.Not.Trim()
        };
        _context.IzlenenDomainler.Add(domain);
        await _context.SaveChangesAsync();

        // Eklenir eklenmez bir kez kontrol et ki pano boş görünmesin.
        await _orkestratör.KontrolEtAsync(domain);
        return RedirectToAction(nameof(Detay), new { id = domain.Id });
    }

    public async Task<IActionResult> Detay(int id)
    {
        var domain = await _context.IzlenenDomainler.FindAsync(id);
        if (domain is null) return NotFound();

        var gecmis = await _context.MonitorChecks
            .Where(c => c.TargetId == id)
            .OrderByDescending(c => c.ZamanUtc)
            .Take(50)
            .ToListAsync();

        return View(new DomainDetayViewModel { Domain = domain, Gecmis = gecmis });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var d = await _context.IzlenenDomainler.FindAsync(id);
        if (d is null) return NotFound();
        return View(new DomainFormViewModel { Id = d.Id, Host = d.Host, Port = d.Port, Aktif = d.Aktif, Not = d.Not });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(DomainFormViewModel vm)
    {
        var host = Normalize(vm.Host);
        if (host.Length == 0)
            ModelState.AddModelError(nameof(vm.Host), "Alan adı geçersiz.");
        else if (await _context.IzlenenDomainler.AnyAsync(d => d.Host == host && d.Port == vm.Port && d.Id != vm.Id))
            ModelState.AddModelError(nameof(vm.Host), "Bu alan adı + port zaten ekli.");

        if (!ModelState.IsValid) return View(vm);

        var d = await _context.IzlenenDomainler.FindAsync(vm.Id);
        if (d is null) return NotFound();

        d.Host = host;
        d.Port = vm.Port;
        d.Aktif = vm.Aktif;
        d.Not = string.IsNullOrWhiteSpace(vm.Not) ? null : vm.Not.Trim();
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Detay), new { id = d.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var d = await _context.IzlenenDomainler.FindAsync(id);
        if (d is null) return NotFound();

        await _context.MonitorChecks.Where(c => c.TargetId == id).ExecuteDeleteAsync();
        _context.IzlenenDomainler.Remove(d);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SimdiKontrolEt(int id)
    {
        var d = await _context.IzlenenDomainler.FindAsync(id);
        if (d is null) return NotFound();

        var sonuc = await _orkestratör.KontrolEtAsync(d);
        TempData["Bilgi"] = $"Kontrol edildi: {sonuc.Mesaj}";
        return RedirectToAction(nameof(Detay), new { id });
    }

    private static string Normalize(string host)
    {
        var h = host.Trim().ToLowerInvariant();
        // "https://example.com/path" gibi girişleri hoş gör.
        if (h.StartsWith("http://")) h = h[7..];
        if (h.StartsWith("https://")) h = h[8..];
        var slash = h.IndexOf('/');
        if (slash >= 0) h = h[..slash];
        var colon = h.IndexOf(':');
        if (colon >= 0) h = h[..colon];
        return h;
    }
}
