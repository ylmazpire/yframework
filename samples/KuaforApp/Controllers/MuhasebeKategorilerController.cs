using KuaforApp.Data;
using KuaforApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YFramework.Auth;

namespace KuaforApp.Controllers;

[Authorize(Roles = Roles.Admin)]
public class MuhasebeKategorilerController : Controller
{
    private readonly AppDbContext _context;

    public MuhasebeKategorilerController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var kategoriler = await _context.MuhasebeKategorileri.OrderBy(k => k.Tur).ThenBy(k => k.Ad).ToListAsync();
        return View(kategoriler);
    }

    [HttpGet]
    public IActionResult Create() => View(new MuhasebeKategorisi());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MuhasebeKategorisi kategori)
    {
        if (!ModelState.IsValid) return View(kategori);

        _context.MuhasebeKategorileri.Add(kategori);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
