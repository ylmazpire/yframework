using System.Diagnostics;
using KuaforApp;
using KuaforApp.Data;
using KuaforApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YFramework.Reporting;

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
        if (User.Identity?.IsAuthenticated != true)
        {
            return View("Welcome");
        }

        if (User.IsInRole(AppRoles.IsletmeAdmini) is false)
        {
            // Platform admini: işletme verisi yerine işletme yönetim ekranına yönlendirilir.
            return RedirectToAction("Index", "Isletmeler");
        }

        var bugun = DateTime.Today;
        var yarin = bugun.AddDays(1);
        var haftaSonu = bugun.AddDays(7);

        var buAyKayitlari = await _context.MuhasebeKayitlari
            .Include(k => k.Kategori)
            .Where(k => k.Tarih.Year == bugun.Year && k.Tarih.Month == bugun.Month)
            .ToListAsync();

        var aylikTrend = await BuildAylikTrendAsync(bugun);

        var model = new DashboardViewModel
        {
            BuAyOzeti = FinancialSummaryCalculator.Ozet(buAyKayitlari),
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
                .ToListAsync(),
            AylikTrend = aylikTrend
        };

        return View(model);
    }

    private async Task<List<AylikTrendNoktasi>> BuildAylikTrendAsync(DateTime bugun)
    {
        var aylar = new[] { "", "Oca", "Şub", "Mar", "Nis", "May", "Haz", "Tem", "Ağu", "Eyl", "Eki", "Kas", "Ara" };

        var baslangicAy = new DateTime(bugun.Year, bugun.Month, 1).AddMonths(-5);
        var kayitlar = await _context.MuhasebeKayitlari
            .Include(k => k.Kategori)
            .Where(k => k.Tarih >= baslangicAy)
            .ToListAsync();

        // 6 aylık gelir/gider trendi artık framework'ten geliyor (bkz. FinancialSummaryCalculator.AylikTrend).
        return FinancialSummaryCalculator.AylikTrend(kayitlar, baslangicAy, 6)
            .Select(nokta => new AylikTrendNoktasi
            {
                AyEtiketi = $"{aylar[nokta.Ay]} {nokta.Yil % 100:D2}",
                Gelir = nokta.Ozet.ToplamGelir,
                Gider = nokta.Ozet.ToplamGider
            })
            .ToList();
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
