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
}
