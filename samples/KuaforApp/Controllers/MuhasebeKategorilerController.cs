using KuaforApp.Data;
using KuaforApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YFramework.MultiTenancy;

namespace KuaforApp.Controllers;

[Authorize(Roles = AppRoles.IsletmeAdmini)]
public class MuhasebeKategorilerController : Controller
{
    private readonly AppDbContext _context;
    private readonly ICurrentTenantProvider _currentTenant;

    public MuhasebeKategorilerController(AppDbContext context, ICurrentTenantProvider currentTenant)
    {
        _context = context;
        _currentTenant = currentTenant;
    }

    public async Task<IActionResult> Index()
    {
        var kategoriler = await _context.MuhasebeKategorileri.OrderBy(k => k.Tur).ThenBy(k => k.Ad).ToListAsync();
        return View(kategoriler);
    }

    [HttpGet]
    public IActionResult Create() => View(new MuhasebeKategorisiCreateViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MuhasebeKategorisiCreateViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var kategori = new MuhasebeKategorisi
        {
            TenantId = _currentTenant.TenantId!.Value,
            Ad = model.Ad,
            Tur = model.Tur,
            Periyot = model.Tur == IslemTuru.Gider ? model.Periyot : GiderPeriyodu.TekSeferlik
        };

        _context.MuhasebeKategorileri.Add(kategori);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
