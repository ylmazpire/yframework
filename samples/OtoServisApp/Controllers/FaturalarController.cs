using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OtoServisApp.Data;
using OtoServisApp.Models;
using YFramework.MultiTenancy;
using YFramework.Reporting;
using YFramework.Sequencing;

namespace OtoServisApp.Controllers;

[Authorize(Roles = AppRoles.ServisAdmini)]
public class FaturalarController : Controller
{
    private readonly AppDbContext _context;
    private readonly ICurrentTenantProvider _currentTenant;
    private readonly ISequenceGenerator _sequence;

    public FaturalarController(AppDbContext context, ICurrentTenantProvider currentTenant, ISequenceGenerator sequence)
    {
        _context = context;
        _currentTenant = currentTenant;
        _sequence = sequence;
    }

    public async Task<IActionResult> Index()
    {
        var faturalar = await _context.Faturalar
            .Include(f => f.IsEmri).ThenInclude(e => e!.Arac).ThenInclude(a => a!.Musteri)
            .OrderByDescending(f => f.Tarih)
            .ToListAsync();
        return View(faturalar);
    }

    public async Task<IActionResult> Detay(int id)
    {
        var fatura = await _context.Faturalar
            .Include(f => f.IsEmri).ThenInclude(e => e!.Arac).ThenInclude(a => a!.Musteri)
            .FirstOrDefaultAsync(f => f.Id == id);
        if (fatura is null) return NotFound();

        var kalemler = await _context.IsEmriKalemleri
            .Where(k => k.IsEmriId == fatura.IsEmriId)
            .OrderBy(k => k.Id)
            .ToListAsync();

        ViewBag.Ozet = InvoiceCalculator.Hesapla(kalemler);
        ViewBag.Kalemler = kalemler;
        return View(fatura);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Olustur(int isEmriId)
    {
        var isEmri = await _context.IsEmirleri.FirstOrDefaultAsync(e => e.Id == isEmriId);
        if (isEmri is null) return NotFound();

        if (isEmri.Durum is not (IsEmriDurumu.Tamamlandi or IsEmriDurumu.TeslimEdildi))
        {
            TempData["Hata"] = "Fatura yalnızca tamamlanmış iş emri için kesilebilir.";
            return RedirectToAction("Detay", "IsEmirleri", new { id = isEmriId });
        }

        if (await _context.Faturalar.AnyAsync(f => f.IsEmriId == isEmriId))
        {
            TempData["Hata"] = "Bu iş emri için zaten fatura kesilmiş.";
            return RedirectToAction("Detay", "IsEmirleri", new { id = isEmriId });
        }

        var kalemler = await _context.IsEmriKalemleri.Where(k => k.IsEmriId == isEmriId).ToListAsync();
        if (kalemler.Count == 0)
        {
            TempData["Hata"] = "İş emrinde kalem yok, fatura kesilemez.";
            return RedirectToAction("Detay", "IsEmirleri", new { id = isEmriId });
        }

        var ozet = InvoiceCalculator.Hesapla(kalemler);
        var tenantId = _currentTenant.TenantId!.Value;
        var yil = DateTime.Now.Year;

        // Kiracı + yıl bazlı sıralı numara. NextAsync kendi SaveChanges'ini yapar; bu yüzden
        // Fatura'yı context'e EKLEMEDEN önce numarayı alıyoruz.
        var sira = await _sequence.NextAsync($"fatura-{tenantId}-{yil}");
        var faturaNo = DocumentNumber.Format(yil.ToString(), sira);

        var fatura = new Fatura
        {
            TenantId = tenantId,
            IsEmriId = isEmriId,
            FaturaNo = faturaNo,
            Tarih = DateTime.Now,
            AraToplam = ozet.AraToplam,
            ToplamKdv = ozet.ToplamKdv,
            GenelToplam = ozet.GenelToplam
        };
        _context.Faturalar.Add(fatura);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // Eşzamanlı ikinci istek: iş emri başına tekil index devreye girdi.
            TempData["Hata"] = "Bu iş emri için fatura az önce oluşturuldu.";
            return RedirectToAction("Detay", "IsEmirleri", new { id = isEmriId });
        }

        return RedirectToAction(nameof(Detay), new { id = fatura.Id });
    }
}
