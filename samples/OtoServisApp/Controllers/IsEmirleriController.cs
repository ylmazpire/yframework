using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OtoServisApp.Data;
using OtoServisApp.Models;
using YFramework.Inventory;
using YFramework.MultiTenancy;
using YFramework.Scheduling;

namespace OtoServisApp.Controllers;

[Authorize(Roles = AppRoles.ServisAdmini)]
public class IsEmirleriController : Controller
{
    private readonly AppDbContext _context;
    private readonly ICurrentTenantProvider _currentTenant;
    private readonly ILogger<IsEmirleriController> _logger;
    private readonly StockLedger _stok;

    public IsEmirleriController(AppDbContext context, ICurrentTenantProvider currentTenant, ILogger<IsEmirleriController> logger, StockLedger stok)
    {
        _context = context;
        _currentTenant = currentTenant;
        _logger = logger;
        _stok = stok;
    }

    // Bir iş emrini "canlı" (planlaması bir servis kanalını meşgul eden) sayan durumlar.
    private static readonly IsEmriDurumu[] CanliDurumlar =
        { IsEmriDurumu.Beklemede, IsEmriDurumu.DevamEdiyor, IsEmriDurumu.Tamamlandi };

    public async Task<IActionResult> Index(string? ara, string filtre = "acik")
    {
        var sorgu = _context.IsEmirleri
            .Include(e => e.Arac).ThenInclude(a => a!.Musteri)
            .Include(e => e.ServisKanali)
            .Include(e => e.AtananPersonel)
            .AsQueryable();

        sorgu = filtre switch
        {
            "tamamlanan" => sorgu.Where(e => e.Durum == IsEmriDurumu.Tamamlandi || e.Durum == IsEmriDurumu.TeslimEdildi),
            "tumu" => sorgu,
            _ => sorgu.Where(e => e.Durum == IsEmriDurumu.Beklemede || e.Durum == IsEmriDurumu.DevamEdiyor)
        };

        if (!string.IsNullOrWhiteSpace(ara))
        {
            sorgu = sorgu.Where(e =>
                e.Arac!.Plaka.Contains(ara) ||
                e.Arac.Musteri!.AdSoyad.Contains(ara) ||
                e.MusteriSikayeti.Contains(ara));
        }

        ViewData["Ara"] = ara;
        ViewData["Filtre"] = filtre;
        return View(await sorgu.OrderByDescending(e => e.GelisTarihi).ToListAsync());
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? aracId)
    {
        await DoldurAracListesi(aracId);
        return View(new IsEmriCreateViewModel { AracId = aracId ?? 0 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(IsEmriCreateViewModel vm)
    {
        if (!await _context.Araclar.AnyAsync(a => a.Id == vm.AracId))
            ModelState.AddModelError(nameof(vm.AracId), "Seçilen araç bulunamadı.");

        if (!ModelState.IsValid)
        {
            await DoldurAracListesi(vm.AracId);
            return View(vm);
        }

        var isEmri = new IsEmri
        {
            TenantId = _currentTenant.TenantId!.Value,
            AracId = vm.AracId,
            MusteriSikayeti = vm.MusteriSikayeti.Trim(),
            GelisKilometresi = vm.GelisKilometresi,
            GelisTarihi = DateTime.Now,
            Durum = IsEmriDurumu.Beklemede
        };
        _context.IsEmirleri.Add(isEmri);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Detay), new { id = isEmri.Id });
    }

    public async Task<IActionResult> Detay(int id)
    {
        var isEmri = await _context.IsEmirleri
            .Include(e => e.Arac).ThenInclude(a => a!.Musteri)
            .Include(e => e.ServisKanali)
            .Include(e => e.AtananPersonel)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (isEmri is null) return NotFound();

        var kalemler = await _context.IsEmriKalemleri
            .Include(k => k.Parca)
            .Where(k => k.IsEmriId == id)
            .OrderBy(k => k.Id)
            .ToListAsync();

        await DoldurPlanlamaListeleri();

        var fatura = await _context.Faturalar.FirstOrDefaultAsync(f => f.IsEmriId == id);

        return View(new IsEmriDetayViewModel
        {
            IsEmri = isEmri,
            Kalemler = kalemler,
            Fatura = fatura,
            YeniKalem = new KalemEkleViewModel { IsEmriId = id },
            Plan = new IsEmriPlanlaViewModel
            {
                Id = id,
                ServisKanaliId = isEmri.ServisKanaliId ?? 0,
                AtananPersonelId = isEmri.AtananPersonelId,
                PlanlananBaslangic = isEmri.PlanlananBaslangic ?? DateTime.Today.AddHours(9),
                PlanlananBitis = isEmri.PlanlananBitis ?? DateTime.Today.AddHours(11)
            }
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Planla(IsEmriPlanlaViewModel vm)
    {
        var isEmri = await _context.IsEmirleri.FindAsync(vm.Id);
        if (isEmri is null) return NotFound();

        var istenen = new TimeRange(vm.PlanlananBaslangic, vm.PlanlananBitis);
        if (!istenen.IsValid)
        {
            TempData["Hata"] = "Bitiş zamanı başlangıçtan sonra olmalı.";
            return RedirectToAction(nameof(Detay), new { id = vm.Id });
        }

        // Serializable: kontrol + yazma aynı transaction'da, iki eşzamanlı planlamanın aynı
        // kanalı aynı saate kapatmasını engellemek için (KuaforApp'teki randevu kalıbının aynısı).
        await using var tx = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        var pencereBaslangic = istenen.Start.AddHours(-24);
        var pencereBitis = istenen.End.AddHours(24);

        var mevcutPlanlar = await _context.IsEmirleri
            .Where(e => e.Id != vm.Id
                        && e.ServisKanaliId != null
                        && e.PlanlananBaslangic != null
                        && e.PlanlananBitis != null
                        && CanliDurumlar.Contains(e.Durum)
                        && e.PlanlananBaslangic < pencereBitis
                        && e.PlanlananBitis > pencereBaslangic)
            .Select(e => new { KanalId = e.ServisKanaliId!.Value, Bas = e.PlanlananBaslangic!.Value, Bit = e.PlanlananBitis!.Value })
            .ToListAsync();

        var aktifKanallar = await _context.ServisKanallari.Where(k => k.Aktif).OrderBy(k => k.Ad).ToListAsync();

        // Framework: N paralel kaynak arasından bu slotta boş olanlar.
        var bosKanallar = ResourceScheduler.AvailableResources(
            aktifKanallar, k => k.Id,
            mevcutPlanlar, p => p.KanalId, p => new TimeRange(p.Bas, p.Bit),
            istenen);

        if (bosKanallar.All(k => k.Id != vm.ServisKanaliId))
        {
            await tx.RollbackAsync();
            var oneri = bosKanallar.Count > 0
                ? " Bu saatte boş kanallar: " + string.Join(", ", bosKanallar.Select(k => k.Ad)) + "."
                : " Bu saatte boş kanal yok.";
            TempData["Hata"] = "Seçilen servis kanalı bu zaman aralığında dolu." + oneri;
            return RedirectToAction(nameof(Detay), new { id = vm.Id });
        }

        isEmri.ServisKanaliId = vm.ServisKanaliId;
        isEmri.AtananPersonelId = vm.AtananPersonelId;
        isEmri.PlanlananBaslangic = istenen.Start;
        isEmri.PlanlananBitis = istenen.End;
        if (isEmri.Durum == IsEmriDurumu.Beklemede)
            isEmri.Durum = IsEmriDurumu.DevamEdiyor;

        await _context.SaveChangesAsync();
        await tx.CommitAsync();

        TempData["Bilgi"] = "İş emri planlandı.";
        return RedirectToAction(nameof(Detay), new { id = vm.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> KalemEkle(KalemEkleViewModel vm)
    {
        var isEmri = await _context.IsEmirleri.FindAsync(vm.IsEmriId);
        if (isEmri is null) return NotFound();

        if (vm.Tur == KalemTuru.Parca && vm.ParcaId is null)
            ModelState.AddModelError(nameof(vm.ParcaId), "Parça kalemi için katalogdan bir parça seçin.");

        if (vm.ParcaId is not null && !await _context.Parcalar.AnyAsync(p => p.Id == vm.ParcaId))
            ModelState.AddModelError(nameof(vm.ParcaId), "Seçilen parça bulunamadı.");

        if (!ModelState.IsValid)
        {
            TempData["Hata"] = "Kalem eklenemedi: " + string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return RedirectToAction(nameof(Detay), new { id = vm.IsEmriId });
        }

        var parcaKalemi = vm.Tur == KalemTuru.Parca;

        _context.IsEmriKalemleri.Add(new IsEmriKalemi
        {
            TenantId = _currentTenant.TenantId!.Value,
            IsEmriId = vm.IsEmriId,
            Tur = vm.Tur,
            ParcaId = parcaKalemi ? vm.ParcaId : null,
            Aciklama = vm.Aciklama.Trim(),
            Adet = vm.Adet,
            BirimFiyat = vm.BirimFiyat,
            KdvOrani = vm.KdvOrani
        });

        // Parça kalemi eklenince stoktan düş (YFramework.Inventory) + hızlı okuma için Parca.StokAdedi güncelle.
        if (parcaKalemi && vm.ParcaId is int parcaId)
        {
            var parca = await _context.Parcalar.FindAsync(parcaId);
            if (parca is not null)
            {
                _stok.Cikis(parcaId, vm.Adet, "İş emri kalemi", $"IsEmri:{vm.IsEmriId}");
                parca.StokAdedi -= (int)Math.Ceiling(vm.Adet);
            }
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Detay), new { id = vm.IsEmriId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> KalemSil(int id, int isEmriId)
    {
        var kalem = await _context.IsEmriKalemleri.FirstOrDefaultAsync(k => k.Id == id && k.IsEmriId == isEmriId);
        if (kalem is null) return NotFound();

        // Parça kalemi silinince stok geri döner.
        if (kalem.Tur == KalemTuru.Parca && kalem.ParcaId is int parcaId)
        {
            var parca = await _context.Parcalar.FindAsync(parcaId);
            if (parca is not null)
            {
                _stok.Giris(parcaId, kalem.Adet, "İş emri kaleminden iade", $"IsEmri:{isEmriId}");
                parca.StokAdedi += (int)Math.Ceiling(kalem.Adet);
            }
        }

        _context.IsEmriKalemleri.Remove(kalem);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Detay), new { id = isEmriId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DurumDegistir(int id, IsEmriDurumu durum)
    {
        var isEmri = await _context.IsEmirleri.FindAsync(id);
        if (isEmri is null) return NotFound();

        isEmri.Durum = durum;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Detay), new { id });
    }

    private async Task DoldurAracListesi(int? secili)
    {
        var araclar = await _context.Araclar.Include(a => a.Musteri).OrderBy(a => a.Plaka).ToListAsync();
        ViewBag.Araclar = new SelectList(
            araclar.Select(a => new { a.Id, Etiket = $"{YFramework.Validation.TurkishLicensePlateFormatter.Bicimlendir(a.Plaka)} — {a.Marka} {a.Model} ({a.Musteri?.AdSoyad})" }),
            "Id", "Etiket", secili);
    }

    private async Task DoldurPlanlamaListeleri()
    {
        ViewBag.Kanallar = new SelectList(
            await _context.ServisKanallari.Where(k => k.Aktif).OrderBy(k => k.Ad).ToListAsync(),
            nameof(ServisKanali.Id), nameof(ServisKanali.Ad));
        ViewBag.Ustalar = new SelectList(
            await _context.Personeller.Where(p => p.Aktif).OrderBy(p => p.AdSoyad).ToListAsync(),
            nameof(Personel.Id), nameof(Personel.AdSoyad));
        ViewBag.Parcalar = new SelectList(
            await _context.Parcalar.OrderBy(p => p.Ad).ToListAsync(),
            nameof(Parca.Id), nameof(Parca.Ad));
    }
}
