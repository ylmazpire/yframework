using KuaforApp.Data;
using KuaforApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YFramework.Auth;

namespace KuaforApp.Controllers;

[Authorize(Roles = Roles.Admin)]
public class HizmetlerController : Controller
{
    private readonly AppDbContext _context;

    public HizmetlerController(AppDbContext context)
    {
        _context = context;
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
            Ad = model.Ad,
            SureDakika = model.SureDakika,
            Fiyat = model.Fiyat
        };

        _context.Hizmetler.Add(hizmet);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
