using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OtoServisApp.Data;
using OtoServisApp.Models;

namespace OtoServisApp.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return View("Welcome");
        }

        ViewData["AcikIsEmri"] = await _context.IsEmirleri
            .CountAsync(e => e.Durum == IsEmriDurumu.Beklemede || e.Durum == IsEmriDurumu.DevamEdiyor);
        ViewData["MusteriSayisi"] = await _context.Musteriler.CountAsync();
        ViewData["AracSayisi"] = await _context.Araclar.CountAsync();
        ViewData["ParcaSayisi"] = await _context.Parcalar.CountAsync();

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
