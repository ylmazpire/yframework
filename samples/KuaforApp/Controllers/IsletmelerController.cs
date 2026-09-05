using KuaforApp.Data;
using KuaforApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YFramework.Auth;

namespace KuaforApp.Controllers;

/// <summary>
/// Platform admininin (sen) işletmeleri ve onların yetkili hesaplarını yönettiği ekran.
/// Uygulamada açık/genel bir kayıt formu yok — her işletme hesabı buradan oluşturulur.
/// </summary>
[Authorize(Roles = Roles.Admin)]
public class IsletmelerController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public IsletmelerController(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var isletmeler = await _context.Isletmeler.OrderBy(i => i.Ad).ToListAsync();
        return View(isletmeler);
    }

    [HttpGet]
    public IActionResult Create() => View(new IsletmeCreateViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(IsletmeCreateViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        if (await _userManager.FindByEmailAsync(model.YetkiliEmail) is not null)
        {
            ModelState.AddModelError(string.Empty, "Bu e-posta ile zaten bir hesap var.");
            return View(model);
        }

        var isletme = new Isletme { Ad = model.IsletmeAdi };
        _context.Isletmeler.Add(isletme);
        await _context.SaveChangesAsync();

        var yetkiliKullanici = new ApplicationUser
        {
            UserName = model.YetkiliEmail,
            Email = model.YetkiliEmail,
            EmailConfirmed = true,
            AdSoyad = model.YetkiliAdSoyad,
            TenantId = isletme.Id
        };

        var result = await _userManager.CreateAsync(yetkiliKullanici, model.YetkiliSifre);
        if (!result.Succeeded)
        {
            _context.Isletmeler.Remove(isletme);
            await _context.SaveChangesAsync();
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View(model);
        }

        await _userManager.AddToRoleAsync(yetkiliKullanici, AppRoles.IsletmeAdmini);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Detay(int id)
    {
        var isletme = await _context.Isletmeler.FindAsync(id);
        if (isletme is null) return NotFound();

        var kullanicilar = await _context.Users.Where(u => u.TenantId == id).ToListAsync();

        return View(new IsletmeDetayViewModel { Isletme = isletme, Kullanicilar = kullanicilar });
    }

    [HttpGet]
    public async Task<IActionResult> SifreSifirla(string kullaniciId)
    {
        var kullanici = await _userManager.FindByIdAsync(kullaniciId);
        // Sadece bir işletmeye bağlı (TenantId dolu) hesapların şifresi buradan sıfırlanabilir —
        // platform admini hesapları (TenantId null) burada hedef alınamaz.
        if (kullanici is null || kullanici.TenantId is null) return NotFound();

        return View(new SifreSifirlaViewModel { KullaniciId = kullanici.Id, Email = kullanici.Email ?? string.Empty });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SifreSifirla(SifreSifirlaViewModel model)
    {
        var kullanici = await _userManager.FindByIdAsync(model.KullaniciId);
        if (kullanici is null || kullanici.TenantId is null) return NotFound();

        if (!ModelState.IsValid)
        {
            model.Email = kullanici.Email ?? string.Empty;
            return View(model);
        }

        await _userManager.RemovePasswordAsync(kullanici);
        var result = await _userManager.AddPasswordAsync(kullanici, model.YeniSifre);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            model.Email = kullanici.Email ?? string.Empty;
            return View(model);
        }

        TempData["Basari"] = $"{kullanici.Email} için şifre güncellendi.";
        return RedirectToAction(nameof(Detay), new { id = kullanici.TenantId });
    }
}
