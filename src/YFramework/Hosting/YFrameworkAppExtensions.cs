using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;

namespace YFramework.Hosting;

/// <summary>
/// yframework ile kurulan her uygulamanın baştan doğru başlaması için bilinen-çalışan
/// bir middleware sırası sağlar. Buradaki her satır, gerçek bir projede yaşanmış bir
/// hatadan öğrenildi — bkz. yorumlar.
/// </summary>
public static class YFrameworkAppExtensions
{
    /// <summary>
    /// Program.cs'te <c>app.UseRouting()</c>'ten ÖNCE çağrılmalıdır.
    /// </summary>
    public static WebApplication UseYFrameworkDefaults(this WebApplication app)
    {
        // .NET'in yeni app.MapStaticAssets() sistemi, tarayıcıların gönderdiği
        // "Accept-Encoding: gzip" header'ı ile gelen isteklerde bazı ortamlarda
        // 0 byte içerik döndürebiliyor (sıkıştırılmış varyant üretilmemiş/bozuksa).
        // curl gibi araçlar bu header'ı varsayılan göndermediği için sorun sadece
        // gerçek tarayıcılarda ortaya çıkıyor ve fark edilmesi çok zor oluyor —
        // sayfa "CSS hiç yüklenmemiş gibi" görünür. Klasik UseStaticFiles() bu
        // sorunu yaşamaz, bu yüzden onu kullanıyoruz.
        app.UseStaticFiles();

        // Form/URL'den gelen ondalık sayılar her zaman InvariantCulture ile yorumlanmalı.
        // HTML5 "number" input'ları tarayıcı dili ne olursa olsun DAİMA nokta (".") ile
        // gönderir (örn. "45.50"). Sunucunun işletim sistemi Türkçe locale kullanıyorsa
        // (nokta = binlik ayracı), bu değer aksi belirtilmedikçe "4550" olarak yanlış
        // parse edilir — parasal alanlarda sessizce veri bozulmasına yol açar.
        // Ekranda para göstermek için ayrı olarak YFramework.Formatting.TurkishCurrencyFormatter
        // kullanılmalı (bu ayar sadece GİRDİ ayrıştırmasını etkiler, görünümü değil).
        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(CultureInfo.InvariantCulture),
            SupportedCultures = new List<CultureInfo> { CultureInfo.InvariantCulture },
            SupportedUICultures = new List<CultureInfo> { CultureInfo.InvariantCulture }
        });

        return app;
    }
}
