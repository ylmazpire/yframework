using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using YFramework.Monitoring;

namespace SslWatchman.Services;

/// <summary>
/// Ağ I/O'sundan bağımsız, saf değerlendirme: elde bir sertifika + TLS politika hataları
/// verildiğinde <see cref="CheckOutcome"/> ve mesaj üretir. Sınır durumları (1 gün önce doldu,
/// tam eşikte, 7 gün uçurumu) burada test edilir.
/// </summary>
public static class SertifikaDegerlendirici
{
    public const int KritikGunEsigi = 7;

    public static SertifikaKontrolSonucu Degerlendir(
        X509Certificate2 sertifika,
        string host,
        SslPolicyErrors politikaHatalari,
        int uyariEsigiGun,
        DateTime simdiUtc)
    {
        var bitisUtc = sertifika.NotAfter.ToUniversalTime();
        var baslangicUtc = sertifika.NotBefore.ToUniversalTime();
        var issuer = KisaIssuer(sertifika.Issuer);
        var san = SanCikar(sertifika);
        var kalanGun = (int)Math.Floor((bitisUtc - simdiUtc).TotalDays);

        SertifikaKontrolSonucu Sonuc(CheckOutcome o, string m) => new(o, m, bitisUtc, issuer, san);

        if (simdiUtc > bitisUtc)
            return Sonuc(CheckOutcome.Critical, $"Sertifika süresi {-kalanGun} gün önce doldu.");

        if (simdiUtc < baslangicUtc)
            return Sonuc(CheckOutcome.Critical, "Sertifika henüz geçerli değil (başlangıç tarihi ileride).");

        if (!sertifika.MatchesHostname(host) || politikaHatalari.HasFlag(SslPolicyErrors.RemoteCertificateNameMismatch))
            return Sonuc(CheckOutcome.Critical, $"Sertifika \"{host}\" alan adını kapsamıyor.");

        if (politikaHatalari.HasFlag(SslPolicyErrors.RemoteCertificateChainErrors))
            return Sonuc(CheckOutcome.Critical, "Sertifika zinciri doğrulanamadı (kendinden imzalı veya güvenilmeyen kök).");

        if (kalanGun < KritikGunEsigi)
            return Sonuc(CheckOutcome.Critical, $"Sertifika {kalanGun} gün sonra doluyor.");

        if (kalanGun < uyariEsigiGun)
            return Sonuc(CheckOutcome.Warning, $"Sertifika {kalanGun} gün sonra doluyor.");

        return Sonuc(CheckOutcome.Ok, $"Geçerli — {kalanGun} gün kaldı ({issuer}).");
    }

    public static string KisaIssuer(string issuer)
    {
        // "CN=R11, O=Let's Encrypt, C=US" → "Let's Encrypt" (varsa O), yoksa CN.
        var parcalar = issuer.Split(',', StringSplitOptions.TrimEntries);
        var o = parcalar.FirstOrDefault(p => p.StartsWith("O=", StringComparison.OrdinalIgnoreCase));
        if (o is not null) return o[2..].Trim('"');
        var cn = parcalar.FirstOrDefault(p => p.StartsWith("CN=", StringComparison.OrdinalIgnoreCase));
        return cn is not null ? cn[3..].Trim('"') : issuer;
    }

    public static IReadOnlyList<string> SanCikar(X509Certificate2 cert)
    {
        foreach (var ext in cert.Extensions)
        {
            if (ext.Oid?.Value != "2.5.29.17") continue; // subjectAltName
            if (ext is X509SubjectAlternativeNameExtension san)
                return san.EnumerateDnsNames().ToList();
        }
        return Array.Empty<string>();
    }
}
