# yframework

Kendi ihtiyaçlarımız için C# / ASP.NET Core üzerine yazılan, tekrar kullanılabilir bir web framework'ü. Amaç: her yeni işletme/ürün fikri için sıfırdan başlamak yerine, burada biriken parçaları (auth, çok kiracılılık, Türkiye'ye özgü doğrulamalar, raporlama vb.) yeniden kullanmak.

## Yapı

```
yframework/
├── src/YFramework/       ← framework'ün kendisi (class library, .NET 10)
└── samples/KuaforApp/    ← yframework'ü kullanan gerçek bir örnek uygulama (kuaför randevu/muhasebe sistemi)
```

`YFramework` bağımsız bir kütüphane; `KuaforApp` ona sadece proje referansı ile bağlı. Yeni bir uygulama yazarken `src/YFramework` klasörünü yeni projeye taşıyıp (veya ileride NuGet paketi haline getirip) aynı şekilde referans verebilirsin.

## YFramework içinde neler var

| Alan | Namespace | Ne işe yarar |
|---|---|---|
| Kimlik doğrulama | `YFramework.Auth` | `ApplicationUser`, Identity tabanlı `YFrameworkIdentityDbContext`, rol tohumlama |
| Çok kiracılılık | `YFramework.MultiTenancy` | `ITenantScoped` + otomatik EF Core sorgu filtresi — her tabloya elle "WHERE TenantId=..." yazmana gerek kalmaz |
| Veri katmanı | `YFramework.Data` | Generic `Repository<TEntity,TKey>` |
| Zamanlama | `YFramework.Scheduling` | `OverlapChecker` — randevu/rezervasyon çakışma kontrolü |
| Raporlama | `YFramework.Reporting` | `FinancialSummaryCalculator` (gelir/gider özeti), `CsvExporter` |
| Biçimlendirme | `YFramework.Formatting` | `TurkishCurrencyFormatter` (₺1.234,56) |
| Doğrulama | `YFramework.Validation` | `TurkishPhoneNumberAttribute` / `TurkishPhoneNumberFormatter` (0 5XX XXX XX XX) |
| Barındırma | `YFramework.Hosting` | `UseYFrameworkDefaults()` — bilinen tuzaklara karşı doğru middleware sırası |

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

## Çok kiracılılık nasıl çalışıyor (özet)

1. `ApplicationUser.TenantId` — `null` ise platform admini (tüm kiracıları görür), doluysa bir işletmeye bağlı kullanıcı.
2. Oturum açarken `TenantClaimsPrincipalFactory` bu bilgiyi bir claim olarak token'a ekler.
3. `ICurrentTenantProvider` bu claim'i okur.
4. `AppDbContext.OnModelCreating` içinde `TenantQueryFilterApplier.Apply(...)` çağrısı, `ITenantScoped` implemente eden HER entity'ye otomatik `WHERE TenantId = @mevcutKiracı` filtresi ekler.

Sonuç: yeni bir tablo eklediğinde sadece `ITenantScoped` implemente et, filtreleme otomatik gelir — unutma riski yok.
