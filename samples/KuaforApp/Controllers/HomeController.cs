using System.Diagnostics;
using KuaforApp.Data;
using KuaforApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KuaforApp.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Admin"))
        {
            return View("Welcome");
        }

        var bugun = DateTime.Today;
        var yarin = bugun.AddDays(1);
        var haftaSonu = bugun.AddDays(7);

        var model = new DashboardViewModel
        {
            ToplamMusteriSayisi = await _context.Musteriler.CountAsync(),
            ToplamHizmetSayisi = await _context.Hizmetler.CountAsync(),
            BugunkuRandevuSayisi = await _context.Randevular
                .CountAsync(r => r.BaslangicZamani >= bugun && r.BaslangicZamani < yarin && r.Durum != RandevuDurumu.IptalEdildi),
            BuHaftakiRandevuSayisi = await _context.Randevular
                .CountAsync(r => r.BaslangicZamani >= bugun && r.BaslangicZamani < haftaSonu && r.Durum != RandevuDurumu.IptalEdildi),
            BugunkuRandevular = await _context.Randevular
                .Include(r => r.Musteri)
                .Include(r => r.Hizmet)
                .Where(r => r.BaslangicZamani >= bugun && r.BaslangicZamani < yarin && r.Durum != RandevuDurumu.IptalEdildi)
                .OrderBy(r => r.BaslangicZamani)
                .ToListAsync()
        };

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
