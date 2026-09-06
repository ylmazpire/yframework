# yframework

Kendi ihtiyaçlarımız için C# / ASP.NET Core üzerine yazılan, tekrar kullanılabilir bir web framework'ü. Amaç: her yeni işletme/ürün fikri için sıfırdan başlamak yerine, burada biriken parçaları (auth, çok kiracılılık, Türkiye'ye özgü doğrulamalar, raporlama vb.) yeniden kullanmak.

## Yapı

```
yframework/
├── src/YFramework/         ← framework'ün kendisi (class library, .NET 10)
├── samples/KuaforApp/      ← örnek uygulama #1 (kuaför randevu/muhasebe sistemi)
└── samples/OtoServisApp/   ← örnek uygulama #2 (oto servis / tamirhane — iş emri, araç, parça/stok)
```

İkinci örnek uygulamanın amacı framework'ü kuaföre özgü varsayımlardan arındırmak: aynı
`src/YFramework`'e farklı bir domain'den bakınca eksik/özelleşmiş kalmış parçalar ortaya çıktı ve
framework'e taşındı — kaynak bazlı planlama (`ResourceScheduler`), KDV'li fatura hesaplama
(`InvoiceCalculator`), plaka/VKN/TCKN doğrulama, kiracı bazlı belge numarası (`Sequencing`),
stok hareketleri (`Inventory`), serbest tarih aralığı + aylık trend raporu.
Eklenen her parçanın geriye dönük uyumlu kaldığı, `KuaforApp` da bu yeni parçaları kullanacak
şekilde güncellenerek doğrulandı (randevu çakışma kontrolü → `ResourceScheduler`, dashboard
trendi → `FinancialSummaryCalculator.AylikTrend`).

`YFramework` bağımsız bir kütüphane; örnek uygulamalar ona sadece proje referansı ile bağlı. Yeni bir uygulama yazarken `src/YFramework` klasörünü yeni projeye taşıyıp (veya ileride NuGet paketi haline getirip) aynı şekilde referans verebilirsin.

## YFramework içinde neler var

| Alan | Namespace | Ne işe yarar |
|---|---|---|
| Kimlik doğrulama | `YFramework.Auth` | `ApplicationUser`, Identity tabanlı `YFrameworkIdentityDbContext`, rol tohumlama |
| Çok kiracılılık | `YFramework.MultiTenancy` | `ITenantScoped` + otomatik EF Core sorgu filtresi — her tabloya elle "WHERE TenantId=..." yazmana gerek kalmaz |
| Veri katmanı | `YFramework.Data` | Generic `Repository<TEntity,TKey>` |
| Zamanlama | `YFramework.Scheduling` | `OverlapChecker` (tek aralık çifti), `TimeRange` (yarı açık [Start,End) + `Overlaps`), `ResourceScheduler` — N paralel kaynak (personel / lift / oda) için "bu slotta hangileri boş?" (`AvailableResources` / `FirstAvailable` / `HasConflict`) |
| Raporlama | `YFramework.Reporting` | `FinancialSummaryCalculator` (gelir/gider özeti + `DonemOzeti` serbest tarih aralığı + `AylikTrend` grafik verisi, `IFinancialTransaction`), `InvoiceCalculator` (satır bazlı KDV → ara toplam + oran bazında KDV kırılımı + genel toplam, `IFaturaSatiri`), `CsvExporter` (`;` ayraç, UTF-8 BOM, CSV formül enjeksiyonu koruması) |
| Belge numarası | `YFramework.Sequencing` | `ISequenceGenerator` / `EfSequenceGenerator` — anahtar bazlı boşluksuz artan numara (fatura/teklif/iş emri no); `DocumentNumber.Format` (`2026-000042`). Kiracıyı bilmez — anahtar `$"fatura-{tenantId}-{yil}"` gibi kurulur. DbContext'te `modelBuilder.AddYFrameworkSequences()` |
| Stok | `YFramework.Inventory` | `StockLedger` — imzalı stok hareketleri (`Giris`/`Cikis`/`EldekiMiktarAsync`/`HareketlerAsync`), her değişimin sebebi + referansı iz kalır. DbContext'te `modelBuilder.AddYFrameworkStockLedger()`. Uygulama isterse hızlı okuma için ayrıca materyalize bir sayaç (ör. `Parca.StokAdedi`) tutabilir |
| Biçimlendirme | `YFramework.Formatting` | `TurkishCurrencyFormatter` (₺1.234,56) |
| Doğrulama | `YFramework.Validation` | Telefon (`TurkishPhoneNumberAttribute` / `...Formatter`), T.C. kimlik no (`TurkishNationalId...`), vergi kimlik no (`TurkishTaxNumber...`), ikisinden biri (`TurkishTaxOrNationalIdAttribute`), plaka (`TurkishLicensePlate...` — normalize + doğrula + `34 ABC 123` biçimi) |
| Barındırma | `YFramework.Hosting` | `UseYFrameworkDefaults()` — bilinen tuzaklara karşı doğru middleware sırası |

Testler: `tests/YFramework.Tests` (xUnit). Doğrulama algoritmaları (TCKN/VKN kontrol haneleri, plaka kombinasyonları) burada test ediliyor — `dotnet test`.

## Yeni bir proje kurarken (`Program.cs`)

```csharp
var app = builder.Build();

app.UseHttpsRedirection();
app.UseYFrameworkDefaults();   // <-- statik dosyalar + kültür ayarı, aşağıya bak
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
```

## Bilinen tuzaklar (buradan öğrendik, tekrar düşmeyelim)

**1. `app.MapStaticAssets()` tarayıcıda CSS/JS'i sessizce boşaltabilir.**
.NET'in yeni statik dosya/sıkıştırma sistemi, tarayıcıların gönderdiği `Accept-Encoding: gzip` header'ı ile gelen isteklerde bazı ortamlarda **200 OK + 0 byte** döndürüyor. `curl` bu header'ı varsayılan göndermediği için terminalde her şey normal görünür, ama gerçek tarayıcıda sayfa komple stilsiz/bozuk açılır — teşhisi çok zordur. Çözüm: `MapStaticAssets()` yerine klasik `app.UseStaticFiles()` kullan. `UseYFrameworkDefaults()` bunu otomatik yapar.

**2. Ondalık sayı girişi, sunucunun işletim sistemi Türkçe locale kullanıyorsa bozulur.**
HTML5 `<input type="number">` tarayıcıdan HER ZAMAN nokta ile gelir (`"45.50"`). Sunucu Türkçe kültürde çalışıyorsa (nokta = binlik ayracı), bu değer aksi belirtilmedikçe `4550` olarak yanlış parse edilir — parasal alanlarda sessiz veri bozulması demektir. Çözüm: `RequestLocalization`'ı `InvariantCulture`'a sabitle (girdi ayrıştırması için), ekranda göstermek için ayrıca `TurkishCurrencyFormatter` kullan. `UseYFrameworkDefaults()` bunu da otomatik yapar.

**3. `dotnet new mvc` şablonundaki varsayılan `_Layout.cshtml.css` dosyasını silmeyi unutma.**
Şablon, eski mavi renk temalı bir "scoped CSS" dosyasıyla gelir. Kendi tasarımını yazdıktan sonra bu dosyayı silmezsen, kullanılmayan kurallar `*.styles.css` paketine sessizce eklenmeye devam eder.

**4. `dotnet run` yerine derlenmiş DLL'i doğrudan çalıştırmak, arka planda süreç kalmasını önler.**
Geliştirme sırasında sunucuyu birden fazla kez başlatıp durdurman gerekiyorsa, `dotnet run` bir üst süreç + alt süreç (gerçek uygulama) çifti oluşturur; üst süreci durdurmak alt süreci öldürmeyebilir. Bu da eski bir sürecin hâlâ aynı SQLite dosyasına bağlı kalıp "silinmiş/değişmiş dosya" hataları vermesine yol açar. `dotnet build` + `dotnet bin/Debug/netX/AppName.dll` ile tek, izlenebilir bir süreç çalıştırmak daha güvenli.

**5. Razor "saçmalıyorsa" (`asp-for`/`asp-items`/`asp-append-version` satırlarında `CS0103: 'sion' yok` gibi anlamsız hatalar) → `dotnet build-server shutdown`.**
Bu ortamda, çözüme yeni bir proje (ör. test projesi) eklenip `restore` çalıştıktan sonra arka plandaki Roslyn derleyici sunucusu (VBCSCompiler) bozuk bir analizör/generator durumuna düşebiliyor. Belirti: tag-helper attribute'ları (`asp-append-version`, `asp-for`, `asp-items`) derlenirken Razor kelimeyi ortadan bölüp yarısını C# tanımlayıcısı sanıyor (`asp-append-ver` + `sion` → `error CS0103: 'sion' ... yok`). Dosyalarda hata YOK; tek projeyi tek başına derlemek çalışır ama `dotnet build <solution>` patlar. Çözüm: `dotnet build-server shutdown` (gerekirse `pkill -f VBCSCompiler`), sonra tekrar derle. `obj/`+`bin/` silmek tek başına yetmez.

## Çok kiracılılık nasıl çalışıyor (özet)

1. `ApplicationUser.TenantId` — `null` ise platform admini (tüm kiracıları görür), doluysa bir işletmeye bağlı kullanıcı.
2. Oturum açarken `TenantClaimsPrincipalFactory` bu bilgiyi bir claim olarak token'a ekler.
3. `ICurrentTenantProvider` bu claim'i okur.
4. `AppDbContext.OnModelCreating` içinde `TenantQueryFilterApplier.Apply(...)` çağrısı, `ITenantScoped` implemente eden HER entity'ye otomatik `WHERE TenantId = @mevcutKiracı` filtresi ekler.

Sonuç: yeni bir tablo eklediğinde sadece `ITenantScoped` implemente et, filtreleme otomatik gelir — unutma riski yok.

**Child (owned) entity'ler:** Bir kayıt yalnızca bir üst kaydın altında yaşıyorsa (ör. `IsEmriKalemi → IsEmri`), EF Core "sahibi kiracı filtreliyse ilişkinin iki ucunda da eşleşen filtre olmalı" uyarısı verir. En temiz çözüm: child'a da `TenantId` koyup `ITenantScoped` yapmak (framework filtreyi + index'i otomatik ekler). Ufak bir denormalizasyon ama child'ı doğrudan sorgulamak da güvenli hale gelir; OtoServisApp bunu böyle yapıyor.
