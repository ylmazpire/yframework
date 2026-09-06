using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OtoServisApp.Data;
using OtoServisApp.Models;
using YFramework.Reporting;

namespace OtoServisApp.Controllers;

[Authorize(Roles = AppRoles.ServisAdmini)]
public class MuhasebeController : Controller
{
    private readonly AppDbContext _context;

    public MuhasebeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(DateTime? bas, DateTime? bit)
    {
        var model = await RaporKur(bas, bit);
        return View(model);
    }

    public async Task<IActionResult> CsvIndir(DateTime? bas, DateTime? bit)
    {
        var model = await RaporKur(bas, bit);

        var headers = new[] { "Tarih", "Tür", "Açıklama", "Yön", "Tutar" };
        var rows = model.Hareketler.Select(h => new[]
        {
            h.Tarih.ToString("yyyy-MM-dd"),
            h.Tur,
            h.Aciklama,
            h.GelirMi ? "Gelir" : "Gider",
            h.Tutar.ToString(CultureInfo.InvariantCulture)
        });

        var csv = CsvExporter.ToCsv(headers, rows);
        var dosyaAdi = $"muhasebe_{model.Baslangic:yyyyMMdd}_{model.Bitis.AddDays(-1):yyyyMMdd}.csv";
        return File(csv, "text/csv", dosyaAdi);
    }

    private async Task<MuhasebeRaporViewModel> RaporKur(DateTime? bas, DateTime? bit)
    {
        var bugun = DateTime.Today;
        var baslangic = (bas ?? new DateTime(bugun.Year, 1, 1)).Date;
        // Bitiş dahil seçilir, sorguda hariç kullanılır.
        var bitisHaric = (bit ?? bugun).Date.AddDays(1);
        if (bitisHaric <= baslangic) bitisHaric = baslangic.AddDays(1);

        var faturalar = await _context.Faturalar
            .Include(f => f.IsEmri).ThenInclude(e => e!.Arac).ThenInclude(a => a!.Musteri)
            .Where(f => f.Tarih >= baslangic && f.Tarih < bitisHaric)
            .ToListAsync();

        var giderler = await _context.Giderler
            .Where(g => g.Tarih >= baslangic && g.Tarih < bitisHaric)
            .ToListAsync();

        var islemler = faturalar.Cast<IFinancialTransaction>().Concat(giderler).ToList();

        var hareketler = faturalar
            .Select(f => new HareketSatiri(
                f.Tarih, "Fatura",
                $"{f.FaturaNo} — {f.IsEmri?.Arac?.Musteri?.AdSoyad}",
                true, f.GenelToplam))
            .Concat(giderler.Select(g => new HareketSatiri(
                g.Tarih, KategoriEtiket(g.Kategori), g.Aciklama, false, g.Tutar)))
            .OrderByDescending(h => h.Tarih)
            .ToList();

        var ayFarki = ((bitisHaric.Year - baslangic.Year) * 12) + bitisHaric.Month - baslangic.Month + 1;
        var aySayisi = Math.Clamp(ayFarki, 1, 24);

        return new MuhasebeRaporViewModel
        {
            Baslangic = baslangic,
            Bitis = bitisHaric.AddDays(-1),
            Ozet = FinancialSummaryCalculator.Ozet(islemler),
            AylikTrend = FinancialSummaryCalculator.AylikTrend(islemler, baslangic, aySayisi),
            Hareketler = hareketler
        };
    }

    private static string KategoriEtiket(GiderKategorisi k) => k switch
    {
        GiderKategorisi.ParcaAlimi => "Parça alımı",
        GiderKategorisi.Kira => "Kira",
        GiderKategorisi.Maas => "Maaş",
        GiderKategorisi.FaturaOdemesi => "Fatura ödemesi",
        _ => "Diğer gider"
    };
}
