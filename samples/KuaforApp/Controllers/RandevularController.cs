using KuaforApp.Data;
using KuaforApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using YFramework.Auth;
using YFramework.Scheduling;

namespace KuaforApp.Controllers;

[Authorize(Roles = Roles.Admin)]
public class RandevularController : Controller
{
    private readonly AppDbContext _context;

    public RandevularController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var randevular = await _context.Randevular
            .Include(r => r.Musteri)
            .Include(r => r.Hizmet)
            .OrderBy(r => r.BaslangicZamani)
            .ToListAsync();
        return View(randevular);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        return View(await BuildViewModelAsync(new RandevuCreateViewModel()));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RandevuCreateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(await BuildViewModelAsync(model));

        var hizmet = await _context.Hizmetler.FindAsync(model.HizmetId);
        if (hizmet is null)
        {
            ModelState.AddModelError(string.Empty, "Seçilen hizmet bulunamadı.");
            return View(await BuildViewModelAsync(model));
        }

        var yeniBaslangic = model.BaslangicZamani;
        var yeniBitis = yeniBaslangic.AddMinutes(hizmet.SureDakika);

        var mevcutRandevular = await _context.Randevular
            .Include(r => r.Hizmet)
            .Where(r => r.Durum != RandevuDurumu.IptalEdildi)
            .ToListAsync();

        var cakisiyorMu = mevcutRandevular.Any(r =>
            OverlapChecker.Overlaps(yeniBaslangic, yeniBitis, r.BaslangicZamani, r.BitisZamani));

        if (cakisiyorMu)
        {
            ModelState.AddModelError(string.Empty, "Bu zaman aralığında başka bir randevu var. Lütfen farklı bir saat seçin.");
            return View(await BuildViewModelAsync(model));
        }

        var randevu = new Randevu
        {
            MusteriId = model.MusteriId,
            HizmetId = model.HizmetId,
            BaslangicZamani = yeniBaslangic,
            Durum = RandevuDurumu.Planlandi
        };

        _context.Randevular.Add(randevu);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Iptal(int id)
    {
        var randevu = await _context.Randevular.FindAsync(id);
        if (randevu is null) return NotFound();

        randevu.Durum = RandevuDurumu.IptalEdildi;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Tamamlandi(int id)
    {
        var randevu = await _context.Randevular
            .Include(r => r.Musteri)
            .Include(r => r.Hizmet)
            .FirstOrDefaultAsync(r => r.Id == id);
        if (randevu is null) return NotFound();

        randevu.Durum = RandevuDurumu.Tamamlandi;

        var gelirZatenVarMi = await _context.MuhasebeKayitlari.AnyAsync(k => k.RandevuId == randevu.Id);
        if (!gelirZatenVarMi && randevu.Hizmet is not null)
        {
            var randevuGeliriKategorisi = await _context.MuhasebeKategorileri
                .FirstOrDefaultAsync(k => k.Ad == "Randevu Geliri" && k.Tur == IslemTuru.Gelir);

            if (randevuGeliriKategorisi is not null)
            {
                _context.MuhasebeKayitlari.Add(new MuhasebeKaydi
                {
                    KategoriId = randevuGeliriKategorisi.Id,
                    Tutar = randevu.Hizmet.Fiyat,
                    Aciklama = $"{randevu.Musteri?.AdSoyad} - {randevu.Hizmet.Ad}",
                    Tarih = randevu.BaslangicZamani.Date,
                    RandevuId = randevu.Id
                });
            }
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task<RandevuCreateViewModel> BuildViewModelAsync(RandevuCreateViewModel model)
    {
        model.Musteriler = await _context.Musteriler
            .OrderBy(m => m.AdSoyad)
            .Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.AdSoyad })
            .ToListAsync();

        model.Hizmetler = await _context.Hizmetler
            .OrderBy(h => h.Ad)
            .Select(h => new SelectListItem { Value = h.Id.ToString(), Text = h.Ad + " (" + h.SureDakika + " dk)" })
            .ToListAsync();

        return model;
    }
}
