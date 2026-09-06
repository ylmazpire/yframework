using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SslWatchman.Data;
using SslWatchman.Models;
using YFramework.MultiTenancy;

namespace SslWatchman.Controllers;

[Authorize(Roles = AppRoles.Yonetici)]
public class AyarlarController : Controller
{
    private readonly AppDbContext _context;
    private readonly ICurrentTenantProvider _currentTenant;

    public AyarlarController(AppDbContext context, ICurrentTenantProvider currentTenant)
    {
        _context = context;
        _currentTenant = currentTenant;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var hesap = await HesabiGetir();
        return View(new AyarlarViewModel { WebhookUrl = hesap.WebhookUrl, UyariEsigiGun = hesap.UyariEsigiGun });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(AyarlarViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var hesap = await HesabiGetir();
        hesap.WebhookUrl = string.IsNullOrWhiteSpace(vm.WebhookUrl) ? null : vm.WebhookUrl.Trim();
        hesap.UyariEsigiGun = vm.UyariEsigiGun;
        await _context.SaveChangesAsync();

        TempData["Bilgi"] = "Ayarlar kaydedildi.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<Hesap> HesabiGetir()
    {
        var id = _currentTenant.TenantId!.Value;
        return await _context.Hesaplar.IgnoreQueryFilters().FirstAsync(h => h.Id == id);
    }
}
